import { createRouter, createWebHistory } from 'vue-router'

import HomeView from '../views/HomeView.vue'
import FloorplannerView from '../views/FloorplannerView.vue'

export const routes = [
    {   
        path: '/', 
        name: 'Home',
        component: HomeView,
        icon: 'mdi-home', 
    },
    { 
        path: '/floorplanner', 
        name: 'Floorplanner123',
        component: FloorplannerView,
        icon: 'mdi-floor-plan',
    },
]

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes,
})

export default router

