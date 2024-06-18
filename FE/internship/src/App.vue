<template>
  <HeaderPage></HeaderPage>
  <div class="main-content">
    <router-view></router-view>
  </div>
  <FooterPage></FooterPage>
</template>

<script>
import HeaderPage from './components/HeaderPage.vue'
import FooterPage from './components/FooterPage.vue'

export default {
  name: 'App',
  components: {
    HeaderPage,
    FooterPage
  }
}
</script>

<style scoped>
.main-content {
  position: relative;
  min-height: 70vh;
}
</style>
