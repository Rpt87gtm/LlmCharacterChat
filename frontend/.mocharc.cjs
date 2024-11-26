module.exports = {
    require: ['@babel/register'],
    spec: ['src/**/__tests__/**/*.test.js'],
    recursive: true,
    reporter: 'spec',
    timeout: 5000
  };