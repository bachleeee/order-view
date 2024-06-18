<template>
  <div class="container mx-auto px-4 max-w-6xl">
    <div class="header-section">
      <div class="flex flex-col justify-center items-center p-5">
        <div class="text-black-500 text-5xl font-bold m-4">Welcome to ANYDAY</div>
        <div class="text-grey-500 text-base m-4">We bring good things to life</div>
        <router-link to="/menu">
          <button class="btn-black">Go to menu</button>
        </router-link>
      </div>
    </div>
    <div class="body-section">
      <div class="m-3" v-for="(item, index) in items" :key="item.id"
        :class="['flex', index % 2 !== 0 ? 'flex-row-reverse' : 'flex-row']">
        <div class="w-3/6 self-center m-5">
          <div class="items-name">{{ item.name }}</div>
          <div class="items-description">{{ item.description }}</div>
          <button class="btn-black" @click="goToDetail(item.id)">Order</button>
        </div>
        <div class="w-3/6">
          <img :src="item.image_url" alt="" class="item-image">
        </div>
      </div>
    </div>
    <div class="footer-section flex flex-wrap mt-6">
      <div v-for="(item, index) in slideItems" :key="index" class="w-1/3 p-4">
        <div>
          <img :src="item.image_url" alt="" class="items-slide-image w-full h-auto">
        </div>
        <div class="my-5">
          <div class="items-slide-name">{{ item.name }}</div>
          <div class="items-slide-description my-2">{{ item.description }}</div>
          <div class="items-slide-price">${{ item.price_m }}</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      items: [],
      slideItems: []
    };
  },
  created() {
    this.fetchItems();
  },
  methods: {
    async fetchItems() {
      try {
        const response = await axios.get('http://localhost:5201/api/Items');
        this.items = response.data.data.items.slice(0, 3);
        this.slideItems = response.data.data.items.slice(4, 7);
      } catch (error) {
        console.error('Error fetching items:', error);
      }
    },
    goToDetail(id) {
      this.$router.push({
        name: 'item-detail',
        params: { itemId: id }
      })
    }
  }
};
</script>

<style scoped>
.header-section {
  margin-bottom: 20px;
}

.body-section {
  display: flex;
  flex-direction: column;
}

.items-name {
  font-weight: bold;
  font-size: 1.5rem;
  margin-bottom: 0.5rem;
}

.items-description {
  margin-bottom: 1rem;
}

.item-image {
  width: 100%;
  height: 300px;
  object-fit: cover;
}

.items-slide-image {
  width: 100%;
  height: 220px;
  object-fit: cover;
}

.items-slide-name {
  font-weight: bold
}

.items-slide-price {
  font-weight: bold
}

.items-slide-description {
  color: rgb(96, 94, 94);
  font-weight: 400;
}
</style>