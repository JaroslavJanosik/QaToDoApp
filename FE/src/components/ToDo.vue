<template>
  <h1>ToDo App</h1>
  <form @submit.prevent="addToDo()">
    <label>New ToDo </label>
    <input
        v-model="newToDo"
        name="newToDo"
        autocomplete="off"
        data-cy="newToDoInput"
    >
    <button data-cy='addToDo'>Add ToDo</button>
  </form>
  <h2>ToDo List</h2>
  <ul>
    <li
        v-for="ToDo in ToDos"
        :key="ToDo.id"
    >
      {{ ToDo.text }}
      <button @click="removeToDo(ToDo.id)">Remove</button>
    </li>
  </ul>
  <h4 v-if="ToDos.length === 0">Empty list.</h4>
</template>

<script lang="ts">
import { ref, Ref } from 'vue';
import axios from 'axios';
import { ToDo } from '@/interfaces/ToDo';

export default {
  name: 'ToDo',

  async setup () {
    const apiUrl = 'https://localhost:5001/api/ToDoitems';

    let ToDos: Ref<Array<ToDo>> = ref([]);
    try {
      const ToDosData = await axios.get<Array<ToDo>>(apiUrl);
      ToDos = ref(ToDosData.data);
    } catch (e) {
      console.error(e);
    }

    const newToDo = ref('');
    const addToDo = async () => {
      if (newToDo.value) {
        try {
          const response = await axios.post<ToDo>(apiUrl, {
            text: newToDo.value,
          });

          ToDos.value.push(response.data);
        } catch (e) {
          console.error(e);
        }
        newToDo.value = '';
      }
    }

    const removeToDo = async (id: number) => {
      try {
        await axios.delete(`${apiUrl}/${id}`);
        const index = ToDos.value.findIndex(t => t.id === id);
        ToDos.value.splice(index, 1);
      } catch (e) {
        console.error(e);
      }
    }

    return {
      ToDos,
      newToDo,
      addToDo,
      removeToDo,
    }
  }
}
</script>

<style lang="scss">
$border: 2px solid
rgba(
    $color: white,
    $alpha: 0.35,
);
$size1: 6px;
$size2: 12px;
$size3: 18px;
$size4: 24px;
$size5: 48px;
$backgroundColor: #27292d;
$textColor: white;
$primaryColor: #a0a4d9;
$secondTextColor: #1f2023;
body {
  margin: 0;
  padding: 0;
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  background-color: $backgroundColor;
  color: $textColor;
  #app {
    max-width: 600px;
    margin-left: auto;
    margin-right: auto;
    padding: 20px;
    h1 {
      font-weight: bold;
      font-size: 28px;
      text-align: center;
    }
    form {
      display: flex;
      flex-direction: column;
      width: 100%;
      label {
        font-size: 14px;
        font-weight: bold;
      }
      input,
      button {
        height: $size5;
        box-shadow: none;
        outline: none;
        padding-left: $size2;
        padding-right: $size2;
        border-radius: $size1;
        font-size: 18px;
        margin-top: $size1;
        margin-bottom: $size2;
      }
      input {
        background-color: transparent;
        border: $border;
        color: inherit;
      }
    }
    button {
      cursor: pointer;
      background-color: $primaryColor;
      border: 1px solid $primaryColor;
      color: $secondTextColor;
      font-weight: bold;
      outline: none;
      border-radius: $size1;
    }
    h2 {
      font-size: 22px;
      border-bottom: $border;
      padding-bottom: $size1;
    }
    ul {
      padding: 10px;
      li {
        display: flex;
        justify-content: space-between;
        align-items: center;
        border: $border;
        padding: $size2 $size4;
        border-radius: $size1;
        margin-bottom: $size2;
        span {
          cursor: pointer;
        }
        .done {
          text-decoration: line-through;
        }
        button {
          font-size: $size2;
          padding: $size1;
        }
      }
    }
    h4 {
      text-align: center;
      opacity: 0.5;
      margin: 0;
    }
  }
}
</style>
