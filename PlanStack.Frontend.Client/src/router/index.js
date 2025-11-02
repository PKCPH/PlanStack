import { createRouter, createWebHistory } from 'vue-router'

import HomeView from '../views/HomeView.vue'
import FloorplannerView from '../views/FloorplannerView.vue'

const routes = [
    {   
        path: '/', 
        name: 'Home',
        component: HomeView 
    },
    { 
        path: '/floorplanner', 
        name: 'Floorplanner',
        component: FloorplannerView
    },
]

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes,
})

export default router

