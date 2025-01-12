<script setup>
// Здесь можно добавить логику, если необходимо
</script>

<template>
  <div class="guide-container">
    <h1 class="guide-title">Добро пожаловать на наш сайт!</h1>
    <p class="guide-subtitle">Здесь вы можете общаться с уникальными персонажами и создавать своих собственных. Давайте разберем, как это работает.</p>

    <section class="guide-section">
      <h2 class="section-title">Как начать общение с персонажем</h2>
      <ol class="step-list">
        <li>Перейдите на страницу <strong>Characters</strong>, нажав соответствующую кнопку в шапке сайта.</li>
        <li>Выберите понравившегося персонажа из списка или создайте своего.</li>
        <li>Нажмите кнопку <strong>Create Chat</strong>, чтобы начать диалог.</li>
        <li>В открывшемся окне чата вводите свои сообщения и отправляйте их персонажу. Все ваши чаты сохраняются на странице <strong>Chats</strong>.</li>
      </ol>
    </section>

    <section class="guide-section">
      <h2 class="section-title">Создание персонажа</h2>
      <p>Чтобы создать своего персонажа, выполните следующие шаги:</p>
      <ol class="step-list">
        <li>В разделе <strong>Characters</strong> нажмите кнопку <strong>Create Character</strong>.</li>
        <li>Заполните поля:
          <div class="field-indent">
            <ul class="tips-list">
              <li><strong>Имя</strong> — укажите имя вашего персонажа.</li>
              <li><strong>System Prompt</strong> — описание персонажа, его характера и поведения.</li>
            </ul>
          </div>
        </li>
      </ol>
    </section>

    <section class="guide-section">
      <h2 class="section-title">Советы по написанию System Prompt</h2>
      <ul class="tips-list">
        <li>Дублируйте имя персонажа в <strong>System Prompt</strong>, так как из поля <strong>Name</strong> оно не передается автоматически.</li>
        <li>Укажите, что персонаж не должен упоминать, что он AI-модель или помощник, чтобы сохранить иммерсивность.</li>
        <li>Разрешите персонажу сочинять факты, если он не знает ответа на вопрос.</li>
        <li>Рекомендуйте персонажу отвечать короткими фразами, чтобы ускорить генерацию текста.</li>
      </ul>
    </section>

    <p class="guide-footer">Теперь вы готовы к увлекательным диалогам! Наслаждайтесь общением и создавайте уникальных персонажей.</p>
  </div>
</template>

<style scoped>
.guide-container {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
  font-family: Arial, sans-serif;
  color: #333;
}

.guide-title {
  font-size: 2.5rem;
  text-align: center;
  margin-bottom: 20px;
  color: #444;
}

.guide-subtitle {
  font-size: 1.2rem;
  text-align: center;
  margin-bottom: 40px;
  color: #666;
}

.guide-section {
  margin-bottom: 40px;
}

.section-title {
  font-size: 1.8rem;
  margin-bottom: 15px;
  color: #555;
}

.step-list {
  list-style: none;
  padding-left: 0;
}

.step-list li {
  align-items: flex-start;
  margin-bottom: 15px;
  line-height: 1.6;
}

.step-number {
  display: inline-block;
  width: 30px;
  height: 30px;
  background-color: #007bff;
  color: #fff;
  border-radius: 50%;
  text-align: center;
  line-height: 30px;
  font-weight: bold;
  margin-right: 10px;
  flex-shrink: 0;
}

.tips-list {
  list-style: none;
  padding-left: 20px;
  margin-top: 10px;
}

.tips-list li {
  margin-bottom: 10px;
  line-height: 1.6;
}

.field-indent {
  margin-top: 10px; 
}

strong {
  margin: 0; 
  padding: 0;
  white-space: nowrap; 
}

.step-list li {
  white-space: normal;
  word-break: keep-all; 
}

.guide-footer {
  text-align: center;
  font-size: 1.1rem;
  color: #777;
  margin-top: 40px;
}
</style>