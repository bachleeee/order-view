<template>
  <div class="container mx-auto px-4 max-w-6xl bg-blue-full">
    <div class="flex py-3 space-x-8">
      <div class="w-2/6 flex-grow">
        <div class="border-solid border-2 border-black-600">
          <div class="flex flex-col justify-center items-center p-5">
            <div class="user-avatar">
              <i class="fa-regular fa-user"></i>
            </div>
            <div class="user-role">
              <span>Role: {{ user.role }}</span>
            </div>
          </div>
        </div>
      </div>
      <div class="w-4/6 flex-grow">
        <div class="border-solid border-2 border-black-600">
          <Form class="w-full max-w-full p-5" :validation-schema="contactFormSchema" @submit="submitForm">
            <div class="md:flex md:items-center mb-6" v-for="field in fields" :key="field.name">
              <div class="w-1/6">
                <label class="block text-black-500 font-bold mb-1 md:mb-0 pr-4" :for="field.name">
                  {{ field.label }}
                </label>
              </div>
              <div class="w-4/6">
                <Field :id="field.name" :name="field.name" :type="field.type"
                  class="bg-white-200 appearance-none border-2 border-gray-200 rounded w-full py-2 px-4 text-gray-700 leading-tight focus:outline-none focus:bg-white focus:border-gray-800"
                  v-model="user[field.name]" :disabled="emailFiled(field.name)" />
                <ErrorMessage :name="field.name" class="text-red-500 text-sm mt-1" />
              </div>
            </div>
            <div class="user-btn-new-img">
              <button type="submit" class="btn-primary">
                <i class="fa-regular fa-floppy-disk mr-3"></i>Save
              </button>
            </div>
          </Form>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import * as yup from 'yup';
import { Form, Field, ErrorMessage } from 'vee-validate';
import axios from 'axios';
import { toast } from 'vue3-toastify';
import 'vue3-toastify/dist/index.css';

export default {
  data() {
    const contactFormSchema = yup.object().shape({
      name: yup.string().required('Name is required'),
      phone: yup
        .string()
        .matches(/^[0-9]+$/, 'Phone number must contain only digits')
        .required('Phone number is required'),
      address: yup.string().required('Address is required')
    });
    return {
      user: {
        role: 'User',
        name: '',
        email: '',
        phone: '',
        address: ''
      },
      contactFormSchema,
      fields: [
        { name: 'name', label: 'Name', type: 'text' },
        { name: 'email', label: 'Email', type: 'email' },
        { name: 'phone', label: 'Phone', type: 'text' },
        { name: 'address', label: 'Address', type: 'text' }
      ]
    };
  },
  components: {
    Form,
    Field,
    ErrorMessage
  },
  methods: {
    submitForm(values) {
      console.log('Form values:', values);
      this.updateUser(values, 4);
    },
    async getUser(id) {
      try {
        const response = await axios.get(`http://localhost:5201/api/Users/${id}`);
        if (response) {
          const responeData = response.data;
          this.user.name = responeData.data.name;
          this.user.phone = responeData.data.phone;
          this.user.email = responeData.data.email;
          this.user.address = responeData.data.address;
        }
      } catch (error) {
        console.error('Error fetching user:', error);
      }
    },
    async updateUser(values, id) {
      try {
        const response = await axios.put(`http://localhost:5201/api/Users/${id}`, values);
        if (response) {
          toast.success('User updated successfully');
        }
      } catch (error) {
        console.error('Error updating user:', error);
        toast.error('Error updating user');
      }
    },
    emailFiled(name) {
      if (name === 'email') return true;
    }
  },
  created() {
    this.getUser(4);
  }
};
</script>

<style scoped>
.user-avatar {
  background-color: #EAE5E5;
  width: 210px;
  height: 210px;
  border-radius: 50%;
  text-align: center;
  font-size: 140px;
}

.user-role {
  font-weight: bold;
  padding: 15px 0;
}

.user-btn-new-img {
  padding: 15px 0;
}

.row {
  display: grid;
  grid-auto-flow: column;
  gap: 5%;
}
</style>
