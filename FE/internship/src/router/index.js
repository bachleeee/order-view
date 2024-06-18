import { createRouter, createWebHistory } from 'vue-router'

import HomeView from '../views/HomeView.vue'
import UserDetail from '../views/UserDetail.vue'
import MenuView from '../views/MenuView.vue'
import ItemDetail from '../views/ItemDetail.vue'
import OrderView from '../views/OrderView.vue'

const routes = [
  {
    path: '/',
    name: 'home',
    component: HomeView
  },
  {
    path: '/menu',
    name: 'menu',
    component: MenuView
  },
  {
    path: '/order',
    name: 'order',
    component: OrderView
  },
  {
    path: '/user-detail',
    name: 'user-detail',
    component: UserDetail
  },
  {
    path: '/item-detail/:itemId',
    name: 'item-detail',
    component: ItemDetail
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
