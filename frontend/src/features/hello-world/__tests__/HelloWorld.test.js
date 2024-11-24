(async () => {
    const chai = await import('chai');
    const expect = chai.default.expect;
    const sinon = await import('sinon');
    const axios = await import('axios');
  
    // Импортируем компонент
    const HelloWorldComponent = require('../../ui/HelloWorld.vue').default;
  
    describe('HelloWorldComponent', () => {
      let vm;
  
      beforeEach(() => {
        console.log('Before each hook'); // Добавляем отладочный вывод
        // Создаем экземпляр компонента
        vm = new (HelloWorldComponent.extend({
          methods: {
            fetchMessage: HelloWorldComponent.methods.fetchMessage
          }
        }))().$mount();
      });
  
      afterEach(() => {
        console.log('After each hook'); // Добавляем отладочный вывод
        sinon.default.restore();
      });
  
      it('should fetch message on mount', async () => {
        console.log('Running test: should fetch message on mount'); // Добавляем отладочный вывод
        // Мокаем axios.get
        const getStub = sinon.default.stub(axios.default, 'get').resolves({ data: 'Hello, World!' });
  
        // Ждем, пока компонент выполнит fetchMessage
        await vm.$nextTick();
  
        expect(getStub.calledOnce).to.be.true;
        expect(vm.message).to.equal('Hello, World!');
      });
  
      it('should handle error when fetching message', async () => {
        console.log('Running test: should handle error when fetching message'); // Добавляем отладочный вывод
        // Мокаем axios.get с ошибкой
        const getStub = sinon.default.stub(axios.default, 'get').rejects(new Error('Network Error'));
  
        // Ждем, пока компонент выполнит fetchMessage
        await vm.$nextTick();
  
        expect(getStub.calledOnce).to.be.true;
        expect(vm.message).to.equal('');
      });
    });
  })();