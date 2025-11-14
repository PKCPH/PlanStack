<template>
  <v-container class="mt-10">
    <h1>Account</h1>

    <div v-if="user">
      <h2>Hello {{ user.username }}</h2>
      <p>Email: {{ user.email }}</p>
      <p>Roles: {{ user.roles.join(', ') }}</p>
    </div>

    <div v-else>
      <p>Loading user info...</p>
    </div>
  </v-container>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { getCurrentUser } from '../components/api/auth.js';

const user = ref(null);

onMounted(() => {
  user.value = getCurrentUser();
  console.log("Current user:", user.value);
});
</script>