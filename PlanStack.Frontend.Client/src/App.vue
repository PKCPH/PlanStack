<template>
  <v-app>
    <v-app-bar app color="blue" dark>
      <v-app-bar-nav-icon @click.stop="drawer = !drawer"></v-app-bar-nav-icon>
      <v-toolbar-title>Floorplan Designer</v-toolbar-title>
    </v-app-bar>

    <v-navigation-drawer 
      v-model="drawer" 
      app
    >
      <v-list-item title="PlanStack" subtitle="Main Menu"></v-list-item>
      <v-divider></v-divider>

      <v-list density="compact" nav>
        <v-list-item
          v-for="item in menuItems"
          :key="item.title"
          :to="item.route"
          :prepend-icon="item.icon"
          :title="item.title"
          router 
          exact
        ></v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-main>
      <RouterView />
    </v-main>
  </v-app>
</template>

<script setup>
import { ref, computed } from 'vue';
import { routes } from './router/index.js'

const drawer = ref(true); 

const menuItems = computed(() => {
    // getting routes from index.js for single source of truth
    return routes
        .filter(route => route.name) 
        .map(route => ({
            title: route.name, 
            route: route.path, 
            icon: route.icon,
        }));
});
</script>