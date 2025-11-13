<template>
  <v-app>
    <v-app-bar app color="primary" dark>
      <v-app-bar-nav-icon @click.stop="drawer = !drawer"></v-app-bar-nav-icon>
      <v-spacer></v-spacer>
      <v-toolbar-title>Planstack</v-toolbar-title>

      <v-app-bar-nav-icon
        v-if="isToolsDrawerAvailable"
        icon
        @click.stop="userRightDrawerState = !userRightDrawerState"
      >
        <v-icon>mdi-cog</v-icon>
      </v-app-bar-nav-icon>
    </v-app-bar>

    <v-navigation-drawer v-model="drawer" app location="left">
      <v-list-item title="Menu"></v-list-item>
      <v-divider></v-divider>

      <v-list density="compact" nav>
        <v-list-item
          v-for="item in menuItems"
          :key="item.title"
          :to="item.route"
          :prepend-icon="item.icon || 'mdi-circle-medium'"
          :title="item.title"
          router
          exact
        ></v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-navigation-drawer
      v-model="showRightDrawer"
      app
      location="right"
      width="300"
    >
      <v-list-item title="Tools"></v-list-item>
      <v-divider></v-divider>

      <v-card-text>
        <div id="right-tools-container"></div>
      </v-card-text>
    </v-navigation-drawer>

    <v-main>
      <RouterView />
    </v-main>
  </v-app>
</template>

<script setup>
import { ref, computed } from "vue";
import { routes } from "./router/index.js";
import { useRoute } from "vue-router";

const drawer = ref(true);
const userRightDrawerState = ref(true);

const route = useRoute();
// used for index.js to determine which views should have a toolsdrawer
const isToolsDrawerAvailable = computed(() => {
  return route.meta.showToolsDrawer === true;
});
const showRightDrawer = computed({
  get() {
    return isToolsDrawerAvailable.value && userRightDrawerState.value;
  },
  set(newValue) {
    userRightDrawerState.value = newValue;
  },
});

const menuItems = computed(() => {
  // getting routes from index.js for single source of truth
  return routes
    .filter((route) => route.name)
    .map((route) => ({
      title: route.name,
      route: route.path,
      icon: route.icon,
    }));
});
</script>
