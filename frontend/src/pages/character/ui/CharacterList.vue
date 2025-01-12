<template>
  <div class="character-list-container">
    <BackButton />
    <h1>Characters</h1>
    <router-link to="/characters/create" class="create-link">Create Character</router-link>

    <!-- Фильтры -->
    <div class="filters">
      <input
        v-model="filters.Name"
        placeholder="Search by name"
        class="filter-input"
      />
      <select v-model="filters.SortBy" class="filter-select">
        <option value="">Sort by</option>
        <option value="name">Name</option>
        <option value="id">ID</option>
      </select>
      <label class="filter-checkbox">
        <input type="checkbox" v-model="filters.IsDescending" />
        Descending
      </label>
      <button @click="applyFilters" class="filter-button">Apply</button>
    </div>

    <!-- Сообщение, если персонажи не найдены -->
    <div v-if="characters.length === 0" class="no-results">
      No characters found.
    </div>

    <!-- Список персонажей -->
    <ul v-else class="character-list">
      <li v-for="character in characters" :key="character.id" class="character-item">
        <router-link :to="`/characters/${character.id}`" class="character-link">
          {{ character.name }}
        </router-link>
      </li>
    </ul>

    <!-- Пагинация -->
    <div class="pagination">
      <button
        @click="goToFirstPage"
        :disabled="pagination.PageNumber === 1"
        class="pagination-button"
      >
        First
      </button>
      <button
        @click="prevPage"
        :disabled="pagination.PageNumber === 1"
        class="pagination-button"
      >
        Previous
      </button>
      <span class="pagination-info">
        Page {{ pagination.PageNumber }} of {{ totalPages }}
      </span>
      <button
        @click="nextPage"
        :disabled="pagination.PageNumber === totalPages"
        class="pagination-button"
      >
        Next
      </button>
      <button
        @click="goToLastPage"
        :disabled="pagination.PageNumber === totalPages"
        class="pagination-button"
      >
        Last
      </button>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted, computed } from 'vue';
import { fetchCharacters, Character } from '@/shared/api/character';
import BackButton from '@/shared/components/BackButton/ui';

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
  components: {
    BackButton,
  },
  setup() {
    const characters = ref<Character[]>([]);
    const totalCount = ref(0);
    const pagination = ref<QueryPage>({ PageNumber: 1, PageSize: 10 });
    const filters = ref<CharacterQuery>({ SortBy: "" });

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

<style scoped>
.character-list-container {
  position: relative;
  padding: 2rem;
  width: 75%;
  margin: 0 auto;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

h1 {
  text-align: center;
  margin-bottom: 1.5rem;
}


.create-link {
  width: 100%;
  padding: 10px;
  margin: 10px;
  background-color: #6C0300;
  color: #fff;
  border: none;
  border-radius: 4px;
  font-size: 16px;
  text-align: center;
  text-decoration: none;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.create-link:hover {
  background-color: #A60400;
  color: #000;
}


.filters {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  margin-top:1rem;
  align-items: center;
  flex-wrap: wrap;
}

.filter-input {
  padding: 0.5rem;
  border: 1px solid #444;
  border-radius: 4px;
  flex: 1;
}

.filter-select {
  padding: 0.5rem;
  border: 1px solid #444;
  border-radius: 4px;
}

.filter-checkbox {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.filter-button {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  color:#fff;
  transition: background-color 0.3s ease;
}

.filter-button:hover {
  background-color: #A60400;
  color:#000;
}

.no-results {
  text-align: center;
  margin-bottom: 2rem;
}

.character-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.character-item {
  margin-bottom: 1rem;
}

.character-link {
  display: block;
  padding: 0.75rem;
  background-color: #6C0300;
  border-radius: 4px;
  color: #fff;
  text-decoration: none;
  transition: background-color 0.3s ease;
}

.character-link:hover {
  color: #000;
  background-color: #A60400;
}

.pagination {
  display: flex;
  justify-content: center;
  gap: 1rem;
  margin-top: 2rem;
}




.pagination-info {
  color: #fff;
  display: flex;
  align-items: center;
}
</style>