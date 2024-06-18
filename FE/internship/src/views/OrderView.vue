<template>
    <div class="container mx-auto px-4 max-w-6xl py-4">
        <div class="flex justify-center pt-4">
            <div class="w-3/6 mr-5">
                <span class="font-semibold text-grey-500 text-lg">Place your order</span>
            </div>
            <div class="w-2/6"></div>
        </div>
        <div class="flex justify-center py-4">
            <div class="w-3/6 mr-5">
                <div class="border-2 border-grey-600 rounded-md p-4 user-infor">
                    <div class="font-bold text-md">Contact information</div>
                    <div class="p-3">
                        <form>
                            <div class="mb-4">
                                <label class="block text-black-500 font-semibold mb-2" for="name">Your name</label>
                                <input class="w-full p-2 border border-gray-300 rounded-lg" type="text" id="name"
                                    v-model="userName" required readonly>
                            </div>
                            <div class="mb-4">
                                <label class="block text-black-500 font-semibold mb-2" for="phone">Your phone</label>
                                <input class="w-full p-2 border border-gray-300 rounded-lg" type="text" id="phone"
                                    v-model="userPhone" required readonly>
                            </div>
                            <div class="">
                                <label class="block text-black-500 font-semibold mb-2" for="address">Your address</label>
                                <input class="w-full p-2 border border-gray-300 rounded-lg" type="text" id="address"
                                    v-model="userAddress" required readonly>
                            </div>
                        </form>
                    </div>
                </div>
                <div class="border-2 border-grey-600 rounded-md p-4 selected-product" style="margin-top: 30px;">
                    <div class="font-bold text-md mb-3">Selected product</div>
                    <div class="flex mb-3">
                        <div class="w-2/6">
                            <span class="uppercase font-semibold text-lg">Product</span>
                        </div>
                        <div class="w-2/6 flex justify-center self-center">
                            <span class="uppercase font-semibold text-lg">Quantity</span>
                        </div>
                        <div class="w-1/6 flex justify-center self-center">
                            <span class="uppercase font-semibold text-lg">Price</span>
                        </div>
                        <div class="w-1/6"></div>
                    </div>
                    <div class="flex my-2" v-for="(item, index) in items" :key="index">
                        <div class="w-2/6 text-gray-600">
                            <div>{{ item.name }}</div>
                            <div class="text-xs">Size {{ item.size }}</div>
                        </div>
                        <div class="w-2/6 text-gray-600 flex justify-center items-center">
                            <i class="fa-solid icon-quatity fa-minus" @click="decreaseQuantity(index, item)"></i>
                            <span>{{ item.quantity }}</span>
                            <i class="fa-solid icon-quatity fa-plus" @click="increaseQuantity(index, item)"></i>
                        </div>
                        <div class="w-1/6 text-gray-600 flex justify-center items-center">
                            ${{ item.price }}
                        </div>
                        <div class="w-1/6 text-red-600 flex justify-end items-center">
                            <i class="fa-solid fa-trash" @click="removeItem(index, item)"></i>
                        </div>
                    </div>
                    <div class="flex mb-3">
                        <div class="w-2/6">
                        </div>
                        <div class="w-2/6 flex justify-center self-center">
                        </div>
                        <div class="w-1/6 flex justify-center self-center">
                        </div>
                        <div class="w-1/6">
                            
                        </div>
                    </div>
                </div>
            </div>
            <div class="w-2/6">
                <div class="border-2 border-grey-600 rounded-md p-4 user-order">
                    <div class="font-bold text-md flex justify-center">Your order</div>
                    <div class="flex">
                        <div class="w-1/2">
                            <div class="font-semibold">Provisional</div>
                            <div class="font-semibold">Discount</div>
                            <div class="font-semibold">Surcharge</div>
                            <div class="font-semibold">Transport fee</div>
                            <div class="font-semibold">Total</div>
                        </div>
                        <div class="w-1/2">
                            <div class="font-semibold flex justify-end">
                                <div>${{ provisional }}</div>
                            </div>
                            <div class="font-semibold flex justify-end">
                                <div>$0.00</div>
                            </div>
                            <div class="font-semibold flex justify-end">
                                <div>$0.00</div>
                            </div>
                            <div class="font-semibold flex justify-end">
                                <div>$0.00</div>
                            </div>
                            <div class="font-semibold flex justify-end">
                                <div>${{ provisional }}</div>
                            </div>
                        </div>
                    </div>
                    <div class="my-5">
                        <textarea class="resize rounded-md w-full"></textarea>
                    </div>
                    <div class="">
                        <button class="btn-checkout w-full" @click="checkout">Checkout</button>
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
            items: [],
            userName: '',
            userPhone: '',
            userAddress: '',
            provisional: 0
        };
    },
    watch: {
        items: {
            handler() {
                this.calculateProvisional();
            },
            deep: true
        }
    },
    methods: {
        async getUser(id) {
            try {
                const response = await axios.get(`http://localhost:5201/api/Users/${id}`);
                if (response) {
                    const responseData = response.data;
                    this.userName = responseData.data.name;
                    this.userPhone = responseData.data.phone;
                    this.userAddress = responseData.data.address;
                }
            } catch (error) {
                console.error('Error fetching user:', error);
            }
        },
        async getUserOrder(id) {
            try {
                const response = await axios.get(`http://localhost:5201/api/Orders/getUserOrder/${id}`);
                if (response) {
                    const responseData = response.data;
                    this.items = responseData.data;
                }
            } catch (error) {
                console.error('Error fetching order:', error);
            }
        },
        calculateProvisional() {
            this.provisional = this.items.reduce((total, item) => {
                return total + (item.price);
            }, 0).toFixed(2);
        },
        async increaseQuantity(index, item) {
            await this.updateItemQuantity(item, "increase");
        },
        async decreaseQuantity(index, item) {
            if (this.items[index].quantity > 1) {
                await this.updateItemQuantity(item, "decrease");
            }
        },
        async removeItem(index, item) {
            try {
                const response = await axios.delete(`http://localhost:5201/api/Orders/user/4/orderDetails/${item.id}/${item.size}`);
                if (response && response.status === 200) {
                    this.items.splice(index, 1);
                }
            } catch (error) {
                console.error('Error removing item:', error);
            }
        },
        async updateItemQuantity(item, action) {
            try {
                const response = await axios.put(`http://localhost:5201/api/Orders/user/4/orderDetails`, {
                    itemId: item.id,
                    quantity: 1,
                    size: item.size,
                    price: item.price/item.quantity,
                    action: action
                });
                if (response && response.status === 200) {
                    console.log('Item quantity updated successfully');
                    const responseData = response.data.data[0]
                    item.price = responseData.Price,
                    item.quantity = responseData.Quantity
                    console.log(responseData)
                }
            } catch (error) {
                console.error('Error updating item quantity:', error);
            }
        },
        checkout() {
            // Handle checkout logic
        }
    },
    created() {
        this.getUser(4);
        this.getUserOrder(4);
    }
};
</script>


<style scoped>
.icon-quatity {
    background-color: #ffffff;
    display: flex;
    width: 24px;
    height: 24px;
    align-items: center;
    justify-content: center;
    margin: 0 30px;
    font-size: 10px;
    color: black;
    border-radius: 50%;
    cursor: pointer;
    border: 1px solid rgb(155, 155, 155);
}
</style>
