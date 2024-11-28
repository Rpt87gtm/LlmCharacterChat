import { createStore } from 'vuex';

export default createStore({
  state: {
    isAuthenticated: false,
  },
  mutations: {
    setAuthenticated(state, value) {
      state.isAuthenticated = value;
    },
  },
  actions: {
    login({ commit }) {
      commit('setAuthenticated', true);
    },
    logout({ commit }) {
      commit('setAuthenticated', false);
    },
  },
});