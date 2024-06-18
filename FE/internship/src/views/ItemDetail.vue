<template>
    <div class="container mx-auto px-4 max-w-6xl py-4">
        <div v-if="item" class="flex justify-center my-5">
            <div class="w-1/2">
                <img :src="item.image_url" alt="" class="item-image w-full h-auto">
            </div>
            <div class="w-1/2 item-detail flex">
                <div class="w-1/6"></div>
                <div class="w-5/6 item-detail">
                    <div class="item-name cursor-pointer">{{ item.name }}</div>
                    <div class="item-price my-3">${{ currentPrice }}</div>
                    <div class="item-description my-2">{{ item.description }}</div>
                    <div class="item-size">
                        <span class="font-medium">Size</span>
                        <div class="flex justify-between mx-4">
                            <label class="container-check">
                                <input type="radio" value="S" v-model="selectedSize" @change="updatePrice">
                                <span class="checkmark"></span> S + ${{ item.price_s }}
                            </label>
                            <label class="container-check">
                                <input type="radio" value="M" v-model="selectedSize" @change="updatePrice"
                                    checked="checked">
                                <span class="checkmark"></span> M + ${{ item.price_m }}
                            </label>
                            <label class="container-check">
                                <input type="radio" value="L" v-model="selectedSize" @change="updatePrice">
                                <span class="checkmark"></span> L + ${{ item.price_l }}
                            </label>
                        </div>
                    </div>
                    <div class="item-quantity my-5">
                        <span class="font-medium">Quantity</span>
                        <div class="flex justify-center self-center">
                            <i class="fa-solid icon-quatity fa-minus" @click="decreaseQuantity"></i>
                            <span>{{ quantity }}</span>
                            <i class="fa-solid icon-quatity fa-plus" @click="increaseQuantity"></i>
                        </div>
                    </div>
                    <div class="item-add">
                        <button class="btn-black-add w-full" @click="addItem(itemsId)">Add to cart - ${{ total
                            }}</button>
                    </div>
                </div>
            </div>
        </div>
        <div class="footer-section">
            <div class="font-semibold text-xl">Related products</div>
            <div class=" flex flex-wrap mt-2">
                <div v-for="(item, index) in slideItems" :key="index" class="w-1/3 p-2 cursor-pointer"
                    @click="goToDetail(item.id)">
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
    </div>
</template>


<script>
import axios from 'axios';

export default {
    data() {
        return {
            item: null,
            itemsId: this.$route.params.itemId,
            selectedSize: 'M',
            quantity: 1,
            currentPrice: 0,
            total: 0,
            slideItems: []
        };
    },
    created() {
        this.fetchItems();
        this.fetchSlideItems();
    },
    watch: {
        '$route.params.itemId': {
            immediate: true,
            handler(newItemId) {
                this.itemsId = newItemId;
                this.fetchItems();
                this.fetchSlideItems();
            }
        }
    },
    methods: {
        async fetchItems() {
            try {
                const response = await axios.get(`http://localhost:5201/api/Items/${this.itemsId}`);
                if (response) {
                    const responseData = response.data;
                    this.item = responseData.data;
                    this.updatePrice();
                }
            } catch (error) {
                console.error('Error fetching items:', error);
            }
        },
        async fetchSlideItems() {
            try {
                const response = await axios.get('http://localhost:5201/api/Items');
                const items = response.data.data.items;
                const itemId = parseInt(this.itemsId);

                if (itemId >= 0) {
                    const slideNumber = itemId;
                    this.slideItems = items.slice(slideNumber, slideNumber + 3);
                } else {
                    console.error('itemId is out of valid range');
                    this.slideItems = [];
                }
            } catch (error) {
                console.error('Error fetching items:', error);
            }
        },
        updatePrice() {
            switch (this.selectedSize) {
                case 'S':
                    this.currentPrice = this.item.price_s;
                    break;
                case 'M':
                    this.currentPrice = this.item.price_m;
                    break;
                case 'L':
                    this.currentPrice = this.item.price_l;
                    break;
            }
            this.calculateTotal();
        },
        increaseQuantity() {
            this.quantity++;
            this.calculateTotal();
        },
        decreaseQuantity() {
            if (this.quantity > 1) {
                this.quantity--;
                this.calculateTotal();
            }
        },
        calculateTotal() {
            this.total = (this.currentPrice * this.quantity).toFixed(2);
        },
        goToDetail(id) {
            this.$router.push({
                name: 'item-detail',
                params: { itemId: id }
            }).then(() => {
                window.scrollTo(0, 0);
            });
        },
        async addItem(itemId) {
            try {
                const orderDetail = {
                    itemId: parseInt(itemId),
                    quantity: this.quantity,
                    size: this.selectedSize,
                    price: parseFloat(this.total)
                };

                const userId = 4;

                const checkResponse = await axios.get(`http://localhost:5201/api/orders/user/${userId}/orderDetails?itemId=${orderDetail.itemId}&size=${orderDetail.size}`);

                if (checkResponse.data.exists) {
                    await axios.put(`http://localhost:5201/api/orders/user/${userId}/orderDetails`, {
                        itemId: orderDetail.itemId,
                        size: orderDetail.size,
                        quantity: orderDetail.quantity,
                        price: parseFloat(this.total),
                        action: "increase"
                    });
                    alert('Item quantity updated successfully');
                } else {
                    const response = await axios.post(`http://localhost:5201/api/orders/user/${userId}/addItem`, orderDetail);
                    if (response.data.status === 200) {
                        alert('Item added to cart successfully');
                    } else {
                        alert('Failed to add item to cart');
                    }
                }
            } catch (error) {
                console.error('Error adding item to order:', error);
            }
        }

    }
};
</script>

<style scoped>
.item-image {
    width: 95%;
    height: 400px;
    object-fit: cover;
    border-radius: 10px;
}

.item-detail {
    margin: 0 0px;
}

.item-name {
    font-weight: bold;
    font-size: 30px;
}

.item-price {
    font-weight: bold;
    font-size: 20px;
}

.item-description {
    color: rgb(96, 94, 94);
    font-weight: 400;
}

.icon-quatity {
    background-color: #d9d9d9;
    display: flex;
    width: 30px;
    height: 30px;
    align-items: center;
    justify-content: center;
    margin: 0 30px;
    font-size: 10px;
    color: black;
    border-radius: 50%;
    cursor: pointer;
}

.container-check {
    display: block;
    position: relative;
    padding-left: 35px;
    margin-bottom: 12px;
    cursor: pointer;
    user-select: none;
    color: #888787;
}

.container-check input {
    position: absolute;
    opacity: 0;
    cursor: pointer;
    height: 0;
    width: 0;
}

.checkmark {
    position: absolute;
    top: 0;
    left: 0;
    height: 25px;
    width: 25px;
    background-color: #eee;
    border-radius: 50%;
}

.container-check:hover input~.checkmark {
    background-color: #ccc;
}

.container-check input:checked~.checkmark {
    background-color: #F1923A;
    color: black;
}

.checkmark:after {
    content: "";
    position: absolute;
    display: none;
}

.container-check input:checked~.checkmark:after {
    display: block;
}

.container-check .checkmark:after {
    top: 9px;
    left: 9px;
    width: 8px;
    height: 8px;
    border-radius: 50%;
    background: white;
}


.items-slide-image {
    width: 100%;
    height: 210px;
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

.footer-section {
    margin: 70px 0;
}
</style>
