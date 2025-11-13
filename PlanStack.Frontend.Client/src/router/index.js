import { createRouter, createWebHistory } from "vue-router";

import HomeView from "../views/HomeView.vue";
import FloorplannerView from "../views/FloorplannerView.vue";
import StandardsView from "../views/StandardsView.vue";
import AccountView from "../views/AccountView.vue";
import ComponentsView from "../views/ComponentsView.vue";
import BuildingStructureView from "../views/BuildingStructureView.vue";

export const routes = [
  {
    path: "/",
    name: "Home",
    component: HomeView,
    icon: "mdi-home",
  },
  {
    path: "/floorplanner",
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
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

export default router;
