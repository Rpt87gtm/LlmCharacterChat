const Mocha = require('mocha');
const path = require('path');

const mocha = new Mocha({
  ui: 'bdd',
  reporter: 'spec'
});

// Добавляем все тестовые файлы
const testDirs = [
  path.join(__dirname, 'src', 'features', 'hello-world', '__tests__'),
  // Добавьте другие папки с тестами по мере необходимости
];
console.log('Test directories:', testDirs);

testDirs.forEach(testDir => {
  const files = require('glob').sync(path.join(testDir, '**/*.test.js'));
  console.log('Found test files:', files); // Добавляем отладочный вывод
  files.forEach(file => {
    mocha.addFile(file);
  });
});

// Запускаем тесты
mocha.run(failures => {
  process.exitCode = failures ? 1 : 0;
});