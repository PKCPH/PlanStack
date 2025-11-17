<template>
  <v-app>
    <v-layout>
      <v-app-bar color="primary">
        <v-app-bar-nav-icon @click.stop="drawer = !drawer"></v-app-bar-nav-icon>

        <v-toolbar-title>Planstack</v-toolbar-title>


          <!-- Auth actions: Logout (if logged in), Register/Login (if not logged in) -->
          <v-list-item
            v-if="isLoggedIn"
            prepend-icon="mdi-logout"
            title="Logout"
            @click="logout"
          ></v-list-item>
          <v-list-item
            v-else
            prepend-icon="mdi-login"
            title="Login"
            :to="'/login'"
            router
            exact
          ></v-list-item>
          <v-list-item
            v-if="!isLoggedIn"
            prepend-icon="mdi-account-plus"
            title="Register"
            :to="'/register'"
            router
            exact
          ></v-list-item>


        <v-app-bar-nav-icon
          v-if="isToolsDrawerAvailable"
          icon
          @click.stop="userRightDrawerState = !userRightDrawerState"
        >
          <v-icon>mdi-cog</v-icon>
        </v-app-bar-nav-icon>
      </v-app-bar>

      <v-navigation-drawer v-model="drawer" location="left" width="250">
        <v-list-item title="Menu"></v-list-item>
        <v-divider></v-divider>

        <v-list density="compact" nav>
          <!-- Main menu items -->
          <v-list-item
            v-for="item in menuItems"
            :key="item.title"
            :to="item.route"
            :prepend-icon="item.icon || 'mdi-circle-medium'"
            :title="item.title"
            router
            exact
          ></v-list-item>

          <!-- Admin Panel subtitle and items -->
          <template v-if="adminPanelItems.length">
            <v-subheader class="admin-panel-subheader">Admin Panel</v-subheader>
            <v-list-item
              v-for="item in adminPanelItems"
              :key="item.title"
              :to="item.route"
              :prepend-icon="item.icon || 'mdi-circle-medium'"
              :title="item.title"
              router
              exact
            ></v-list-item>
          </template>

        </v-list>
      </v-navigation-drawer>

      <v-navigation-drawer
        v-model="showRightDrawer"
        location="right"
        width="350"
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
    </v-layout>
  </v-app>
</template>
<script setup>
import { ref, computed } from "vue";
import { routes } from "./router/index.js";
import { useRoute } from "vue-router";
import { useRouter } from "vue-router";
import { isLoggedIn, setToken, getCurrentUser } from "./components/api/auth.js";

const drawer = ref(true);
const userRightDrawerState = ref(true);

const route = useRoute();
const router = useRouter();

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

const hiddenForLoggedOut = [
  "Components",
  "Account",
  "Building Structures",
  "Standards",
  "Projects",
  "UserAdmin",
];

const hiddenAlways = [
  "Unauthorized", 
  "Floorplans", 
  "Rulesets", 
  "Register", 
  "Login", 
  "SessionTimeout"
];

const hiddenForUser = [
  "Components",
  "Building Structures",
  "UserAdmin",
];

const menuItems = computed(() => {
  // Main menu: exclude hiddenAlways, hiddenForUser, and admin-only items
  const user = getCurrentUser();
  return routes
    .filter((route) => {
      if (!route.name) return false;
      if (hiddenAlways.includes(route.name)) return false;
      if (hiddenForUser.includes(route.name)) return false;
      if (!isLoggedIn.value && hiddenForLoggedOut.includes(route.name)) return false;
      return true;
    })
    .map((route) => ({
      title: route.name,
      route: route.path,
      icon: route.icon,
    }));
});

const adminPanelItems = computed(() => {
  const user = getCurrentUser();
  if (!user || !user.roles || !user.roles.includes('Admin')) return [];
  // Only show hiddenForUser routes for admin
  return routes
    .filter((route) => {
      if (!route.name) return false;
      if (!hiddenForUser.includes(route.name)) return false;
      if (hiddenAlways.includes(route.name)) return false;
      if (!isLoggedIn.value && hiddenForLoggedOut.includes(route.name)) return false;
      return true;
    })
    .map((route) => ({
      title: route.name,
      route: route.path,
      icon: route.icon,
    }));
});

const logout = () => {
  setToken(null);
  router.push("/");
};
</script>
<style scoped>
.admin-panel-subheader {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
  font-weight: bold;
  padding-left: 0;
  padding-right: 0;
  margin-top: 12px;
  margin-bottom: 12px;
}
</style>
