@echo off
start "Backend" cmd /k "cd backend\bin\Release\net8.0 && llmChat.exe"

start "Frontend" cmd /k "cd frontend && npm run dev"

start "Python Model" cmd /k "cd pythonModel && python main.py"

echo Backend, Frontend, and Python Model started successfully!
exit