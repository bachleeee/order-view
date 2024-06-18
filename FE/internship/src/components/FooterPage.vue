<template>
  <div class="container mx-auto px-4 max-w-6xl">
    <div class="flex py-3">
      <div class="w-1/2">
        <span @click="goHome()" class="text-lg uppercase cursor-pointer">Anyday</span>
      </div>
      <div class="w-1/2">
        <div class="flex space-x-4">
          <div class="flex-1">
            <div class="font-bold mb-2">
              About
            </div>
            <div class="mb-1">Company</div>
            <div class="mb-1">Team</div>
            <div class="mb-1">Careers</div>
          </div>
          <div class="flex-1">
            <div class="font-bold mb-2">
              Services
            </div>
            <div class="mb-1">Consulting</div>
            <div class="mb-1">Sales</div>
            <div class="mb-1">Support</div>
          </div>
          <div class="flex-1">
            <div class="font-bold mb-2">
              Contact
            </div>
            <div class="mb-1">Email</div>
            <div class="mb-1">Phone</div>
            <div class="mb-1">Address</div>
          </div>
        </div>
      </div>
    </div>
    <div class="flex py-3">
      <div class="text-xl text-gray-500 space-x-2">
        <i class="fa-brands fa-facebook"></i>
        <i class="fa-brands fa-linkedin"></i>
        <i class="fa-brands fa-youtube"></i>
        <i class="fa-brands fa-instagram"></i>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  methods: {
    goHome() {
      this.$router.push({ name: 'home' });
    }
  }
};
</script>