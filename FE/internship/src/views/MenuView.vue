<template>
    <div class="container mx-auto px-4 max-w-6xl py-4">
        <div class="search-section">
            <div class="flex justify-end">
                <div class="inset-y-0 right-0">
                    <form class="max-w-xs">
                        <div class="relative">
                            <input type="search" id="default-search"
                                class="block w-full p-3 pe-11 ps-11 text-sm text-gray-900 border border-gray-300 rounded-lg bg-gray-50 focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                                placeholder="What would you like to eat?" required v-model="searchQuery"
                                @input="fetchItems" />
                            <div class="absolute inset-y-0 start-0 flex items-center ps-3 pointer-events-none">
                                <svg class="w-4 h-4 text-gray-500 dark:text-gray-400" aria-hidden="true"
                                    xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 20 20">
                                    <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round"
                                        stroke-width="2" d="m19 19-4-4m0-7A7 7 0 1 1 1 8a7 7 0 0 1 14 0Z" />
                                </svg>
                            </div>
                            <div class="absolute inset-y-0 right-0 flex items-center p-3 cursor-pointer"
                                @click="showFilterBar">
                                <i class="fa-solid fa-filter"></i>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
            <div v-if="isShowFileterBar" class="filter-bar mt-4 p-4 border rounded-lg bg-gray-100">
                <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between">
                    <div class="flex items-center mb-4 sm:mb-0">
                        <label for="category" class="mr-2">Category:</label>
                        <select id="category" v-model="categorySelected" @change="applyFilters"
                            class="p-2 border rounded-lg bg-white">
                            <option value="">All</option>
                            <option v-for="category in categoryData" :key="category.id" :value="category.name">
                                {{ category.name }}
                            </option>
                        </select>
                    </div>
                    <div class="flex items-center mb-4 sm:mb-0">
                        <label for="priceMin" class="mr-2">Price Min:</label>
                        <input type="number" id="priceMin" v-model="priceMin" @input="applyFilters"
                            class="p-2 border rounded-lg bg-white">
                    </div>
                    <div class="flex items-center">
                        <label for="priceMax" class="mr-2">Price Max:</label>
                        <input type="number" id="priceMax" v-model="priceMax" @input="applyFilters"
                            class="p-2 border rounded-lg bg-white">
                    </div>
                </div>
            </div>
        </div>
        <div class="menu-section flex flex-wrap mt-6">
            <div v-for="(item, index) in items" :key="index" class="w-1/3 p-4">
                <div>
                    <img :src="item.image_url" alt="" class="item-image w-full h-auto">
                </div>
                <div class="my-5" @click="goToDetail(item.id)">
                    <div class="items-name cursor-pointer">{{ item.name }}</div>
                    <div class="items-description my-2">{{ item.description }}</div>
                    <div class="items-price">${{ item.price_m }}</div>
                </div>
            </div>
        </div>
        <div class="pagination-section flex justify-center my-4">
            <ul class="flex items-center">
                <li :class="{ disabled: currentPage === 1 }">
                    <div class="page-item-arrow" @click="changePage(currentPage - 1)" :disabled="currentPage === 1">
                        <i class="fa-solid fa-chevron-left"></i>
                    </div>
                </li>
                <li v-for="(page, index) in displayedPages" :key="index">
                    <div :class="isThisCurrentPage(page)" class="page-item" @click="changePage(page)">
                        {{ page }}
                    </div>
                </li>
                <li :class="{ disabled: currentPage === totalPages }">
                    <div class="page-item-arrow" @click="changePage(currentPage + 1)"
                        :disabled="currentPage === totalPages">
                        <i class="fa-solid fa-chevron-right"></i>
                    </div>
                </li>
            </ul>
        </div>
    </div>
</template>

<script>
import axios from 'axios';

export default {
    data() {
        return {
            items: [],
            searchQuery: '',
            currentPage: 1,
            itemsPerPage: 9,
            totalPages: 0,
            totalRecords: 0,
            priceMin: 0,
            priceMax: 100,
            categoryData: [],
            isShowFileterBar: false,
            categorySelected: '',
        };
    },
    created() {
        this.fetchItems();
        this.fetchCategory();
    },
    computed: {
        displayedPages() {
            const total = this.totalPages;
            if (total <= 3) {
                return Array.from({ length: total }, (_, i) => i + 1);
            } else {
                const pages = [];
                if (this.currentPage === this.totalPages) {
                    pages.push(this.currentPage - 2, this.currentPage - 1, this.currentPage)
                } else if (this.currentPage === this.totalPages - 1) {
                    pages.push(this.currentPage - 1, this.currentPage, this.currentPage + 1)
                } else {
                    pages.push(this.currentPage, this.currentPage + 1, this.currentPage + 2);
                }
                return pages;
            }
        }
    },
    methods: {
        async fetchItems() {
            try {
                const response = await axios.post(`http://localhost:5201/api/Items/search/?page=${this.currentPage}`, {
                    name: this.searchQuery,
                    category: this.categorySelected,
                    priceMin: this.priceMin,
                    priceMax: this.priceMax,
                });
                const data = response.data.data;
                this.items = data.items;
                this.totalRecords = data.totalRecords;
                this.totalPages = data.totalPages;
            } catch (error) {
                console.error('Error fetching items:', error);
            }
        },
        async fetchCategory() {
            try {
                const response = await axios.get('http://localhost:5201/api/Category');
                if (response) {
                    const data = response.data.data;
                    this.categoryData = data.category;
                }
            } catch (error) {
                console.error('Error fetching category:', error);
            }
        },
        changePage(page) {
            if (page >= 1 && page <= this.totalPages) {
                this.currentPage = page;
                this.fetchItems();
            }
        },
        isThisCurrentPage(page) {
            return page === this.currentPage ? 'currentPage' : '';
        },
        showFilterBar() {
            this.isShowFileterBar = !this.isShowFileterBar;
        },
        applyFilters() {
            this.currentPage = 1;
            this.fetchItems();
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
.search-section {
    margin: 0 15px;
}

.pagination-section button {
    background-color: #1f2937;
    color: white;
    padding: 0.5rem 1rem;
    border: none;
    cursor: pointer;
    margin: 0 0.5rem;
    border-radius: 0.25rem;
}

.pagination-section button:disabled {
    background-color: #6b7280;
    cursor: not-allowed;
}

.item-image {
    width: 100%;
    height: 200px;
    object-fit: cover;
}

.items-name {
    font-weight: bold
}

.items-price {
    font-weight: bold
}

.items-description {
    color: rgb(96, 94, 94);
    font-weight: 400;
}


.page-item {
    width: 35px;
    height: 35px;
    border: 1px solid #D1D5DB;
    align-items: center;
    display: flex;
    justify-content: center;
    color: #323131;
    cursor: pointer;
}

.page-item-arrow {
    width: 35px;
    height: 35px;
    border: 1px solid #D1D5DB;
    align-items: center;
    display: flex;
    justify-content: center;
    color: #6e6a6a;
    cursor: pointer;
    font-size: 15px;
}

.page-item:hover {
    border: 1px solid #D1D5DB;
    background-color: #007BFF;
    color: white;
}

.currentPage {
    border: 1px solid #D1D5DB;
    background-color: #007BFF;
    color: white;
}
</style>