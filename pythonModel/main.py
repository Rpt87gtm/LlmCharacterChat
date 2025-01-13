from fastapi import FastAPI, WebSocket, BackgroundTasks, HTTPException
from typing import List, Dict, Any
from pydantic import BaseModel
from gpt4all import GPT4All  
from asyncio import Queue


app = FastAPI()
model = None

class Message(BaseModel):
    role: str  
    content: str


class ChatRequest(BaseModel):
    messages: List[Message]
    system_prompt: str
    streaming: bool = False

class TokenStreamQueue:
    def __init__(self):
        self.queue = Queue()

    async def put(self, token: str):
        await self.queue.put(token)

    async def get(self):
        return await self.queue.get()

token_streams: Dict[str, TokenStreamQueue] = {}

@app.on_event("startup")
async def load_model():
    global model
    model = GPT4All("D:/025-LLMGames/LLMGameCore/LLMGameCore/models/nous-hermes-2-solar-10.7b.Q4_0.gguf",device="cpu")  
    print("Модель загружена!")

@app.on_event("shutdown")
async def shutdown_model():
    global model
    model = None
    print("Модель выгружена!")


async def generate_response(messages: List[Message], system_prompt: str, queue: TokenStreamQueue = None):
    history = [{"role": "system", "content": system_prompt}] + [msg.model_dump() for msg in messages[:-1]]
    userMessage = messages[-1].content

    model._history = history
    
    model._current_prompt_template = "### Human:\n{0}\n\n### Assistant:\n"
    

    with model.chat_session(system_prompt=history_to_string(history), prompt_template="### Human:\n{0}\n\n### Assistant:\n"):
        print(model.chat_session)
        if queue:                                                              
            for token in model.generate(userMessage, streaming=True):
                await queue.put(token)
            await queue.put("__END__")
            model._history = None
            model._current_prompt_template = "{0}"
        else:
            print("start generate")
            response = model.generate(userMessage)
            print("AFTER GENERATION:")
            print(model._history)
            model._history = None
            model._current_prompt_template = "{0}"
            return response
        
def history_to_string(history: List[Dict[str, str]]) -> str:
    result = []
    for message in history:
        role = message["role"]
        content = message["content"]
        result.append(f"{role.capitalize()}: {content}")
    return "\n".join(result)

@app.post("/chat")
async def chat(request: ChatRequest):
    if not model:
        raise HTTPException(status_code=503, detail="Модель не загружена")

    if request.streaming:
        return HTTPException(status_code=400, detail="Используйте WebSocket для стриминга токенов")

    response = await generate_response(request.messages, request.system_prompt)
    return {"response": response}


@app.websocket("/chat/stream")                                                #TODO rewrite on manual context control and test
async def websocket_endpoint(websocket: WebSocket):
    await websocket.accept()
    stream_id = id(websocket)
    queue = TokenStreamQueue()
    token_streams[stream_id] = queue

    try:
        while True:
            data = await websocket.receive_json()
            messages = [Message(**msg) for msg in data.get("messages", [])]

            background_tasks = BackgroundTasks()
            background_tasks.add_task(generate_response, messages, queue)
            await background_tasks()

            while True:
                token = await queue.get()
                if token == "__END__":
                    break
                await websocket.send_text(token)
    except Exception as e:
        print(f"Ошибка WebSocket: {e}")
    finally:
        del token_streams[stream_id]
        await websocket.close()

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)