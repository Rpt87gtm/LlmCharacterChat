<template>
  <div>
    <h1>Characters</h1>
    <router-link to="/characters/create">Create Character</router-link>

    <!-- Фильтры -->
    <div>
      <input v-model="filters.Name" placeholder="Search by name" />
      <select v-model="filters.SortBy">
        <option value="">Sort by</option>
        <option value="name">Name</option>
        <option value="id">ID</option>
      </select>
      <label>
        <input type="checkbox" v-model="filters.IsDescending" />
        Descending
      </label>
      <button @click="applyFilters">Apply</button>
    </div>

    <!-- Сообщение, если персонажи не найдены -->
    <div v-if="characters.length === 0">
      No characters found.
    </div>

    <!-- Список персонажей -->
    <ul v-else>
      <li v-for="character in characters" :key="character.id">
        <router-link :to="`/characters/${character.id}`">{{ character.name }}</router-link>
      </li>
    </ul>

    <!-- Пагинация -->
    <div>
      <button @click="goToFirstPage" :disabled="pagination.PageNumber === 1">First</button>
      <button @click="prevPage" :disabled="pagination.PageNumber === 1">Previous</button>
      <span>Page {{ pagination.PageNumber }} of {{ totalPages }}</span>
      <button @click="nextPage" :disabled="pagination.PageNumber === totalPages">Next</button>
      <button @click="goToLastPage" :disabled="pagination.PageNumber === totalPages">Last</button>
    </div>
  </div>
</template>
  
<script lang="ts">
import { defineComponent, ref, onMounted, computed } from 'vue';
import { fetchCharacters, Character } from '@/shared/api/character';

interface CharacterQuery {
  Name?: string;
  SortBy?: string;
  IsDescending?: boolean;
}

interface QueryPage {
  PageNumber: number;
  PageSize: number;
}

export default defineComponent({
  name: 'CharacterList',
  setup() {
    const characters = ref<Character[]>([]);
    const totalCount = ref(0);
    const pagination = ref<QueryPage>({ PageNumber: 1, PageSize: 10 });
    const filters = ref<CharacterQuery>({});

    const totalPages = computed(() => Math.ceil(totalCount.value / pagination.value.PageSize));

    const loadCharacters = async () => {
      const response = await fetchCharacters(filters.value, pagination.value);
      characters.value = response.data;
      totalCount.value = response.totalCount;
    };

    const applyFilters = () => {
      pagination.value.PageNumber = 1; 
      loadCharacters();
    };

    const nextPage = () => {
      if (pagination.value.PageNumber < totalPages.value) {
        pagination.value.PageNumber += 1;
        loadCharacters();
      }
    };

    const prevPage = () => {
      if (pagination.value.PageNumber > 1) {
        pagination.value.PageNumber -= 1;
        loadCharacters();
      }
    };

    const goToFirstPage = () => {
      pagination.value.PageNumber = 1;
      loadCharacters();
    };

    const goToLastPage = () => {
      pagination.value.PageNumber = totalPages.value;
      loadCharacters();
    };

    onMounted(loadCharacters);

    return {
      characters,
      filters,
      pagination,
      totalPages,
      applyFilters,
      nextPage,
      prevPage,
      goToFirstPage,
      goToLastPage,
    };
  },
});
</script>