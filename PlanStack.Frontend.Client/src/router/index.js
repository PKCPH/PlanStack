import { createRouter, createWebHistory } from "vue-router";
import { getCurrentUser } from "../components/api/Auth.js";

import HomeView from "../views/HomeView.vue";
import FloorplannerView from "../views/FloorplannerView.vue";
import StandardsView from "../views/StandardsView.vue";
import AccountView from "../views/AccountView.vue";
import ComponentsView from "../views/ComponentsView.vue";
import BuildingStructureView from "../views/BuildingStructureView.vue";
import RegisterView from "../views/RegisterView.vue";
import LoginView from "../views/LoginView.vue";
import ProjectView from "../views/ProjectView.vue";
import UnauthorizedView from "../views/UnauthorizedView.vue";
import RulesetView from "../views/RulesetView.vue";

export const routes = [
  {
    path: "/",
    name: "Home",
    component: HomeView,
    icon: "mdi-home",
  },
  {
    path: "/projects",
    name: "Projects",
    component: ProjectView,
    icon: "mdi-lightbulb-on-outline",
    meta: { showToolsDrawer: true },
  },
  {
    path: "/project/:projectId/floorplans",
    name: "Floorplans",
    component: FloorplannerView,
    icon: "mdi-floor-plan",
    meta: { showToolsDrawer: true },
  },
  {
    path: "/standards",
    name: "Standards",
    component: StandardsView,
    icon: "mdi-pencil-ruler",
  },
  {
    path: "/components",
    name: "Components",
    component: ComponentsView,
    icon: "mdi-palette-swatch-variant",
  },
  {
    path: "/buildingstructures",
    name: "Building Structures",
    component: BuildingStructureView,
    icon: "mdi-wrench-outline",
  },
  {
    path: "/account",
    name: "Account",
    component: AccountView,
    icon: "mdi-account",
  },
  {
    path: "/register",
    name: "Register",
    component: RegisterView,
    icon: "mdi-account",
  },
  {
    path: "/login",
    name: "Login",
    component: LoginView,
    icon: "mdi-account",
  },
  {
    path: "/unauthorized",
    name: "Unauthorized",
    component: UnauthorizedView,
  },
  {
    path: "/standard/:standardId/rulesets",
    name: "Rulesets",
    component: RulesetView,
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

router.beforeEach((to, from, next) => {
  const publicPages = ["Login", "Register", "Home"];
  const authRequired = !publicPages.includes(to.name);
  const user = getCurrentUser();

  // if (to.name === 'Components' && (!user || !user.roles.includes('Admin'))) {
  //   return next({ name: 'Unauthorized' });
  // }

  // if (to.name === 'Building Structures' && (!user || !user.roles.includes('Admin'))) {
  //   return next({ name: 'Unauthorized' });
  // }

  // if (authRequired && !user) {
  //   return next({ name: 'Login' });
  // }
  next();
});

export default router;
