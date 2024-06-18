<template>
    <div class="shadow-lg">
        <div class="container mx-auto p-3 max-w-7xl">
            <div class="flex py-3">
                <div class="w-1/6">
                    <span @click="goHome()" class="text-lg uppercase cursor-pointer">Anyday</span>
                </div>
                <div class="w-5/6">
                    <div class="flex justify-end items-center space-x-7 relative text-2xl">
                        <router-link to="/" class="text-base">Home</router-link>
                        <router-link to="/menu" class="text-base">Menu</router-link>
                        <router-link to="/order"><i class="fa-solid fa-cart-shopping"></i></router-link>
                        <i @click="changePullDown" class="cursor-pointer fa-solid fa-circle-user"></i>
                    </div>
                </div>
            </div>
            <div v-if="isShowPullDown"
                class="menu-pulldown absolute top-12 right-24 bg-white border border-gray-300 p-2 shadow-lg z-50">
                <div @click="goUserDetail()" class="py-1 px-2 cursor-pointer hover:bg-gray-100">Detail</div>
                <div class="py-1 px-2 cursor-pointer hover:bg-gray-100">Logout</div>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    data() {
        return {
            isShowPullDown: false
        };
    },
    methods: {
        changePullDown() {
            this.isShowPullDown = !this.isShowPullDown;
        },
        goHome() {
            this.$router.push({ name: 'home' })
        },
        goUserDetail() {
            this.$router.push({ path: '/user-detail' })
        }
    }
};
</script>

<style scoped>
.menu-pulldown {
    position: absolute;
    top: 50px;
    right: 100px;
    background-color: white;
    border: 1px solid #ddd;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
    z-index: 1000;
}

.menu-pulldown div {
    padding: 2px 10px;
    cursor: pointer;
}

.menu-pulldown div:hover {
    background-color: #f0f0f0;
}


</style>
