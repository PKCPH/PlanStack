import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from  './router/index.js'
import 'vuetify/styles'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import '@mdi/font/css/materialdesignicons.css'
import colors from 'vuetify/util/colors'

const vuetify = createVuetify({
    components,
    directives,
    icons: {
        defaultSet: 'mdi',
    },
    theme: {
        themes: {
            light: {
                dark: false,
                colors: {
                primary: colors.blueGrey.darken4,
                secondary: colors.indigo.lighten1,
                accent: colors.amber.base,
                background: colors.grey.lighten5
                }
            },
        },
    },
})

createApp(App).use(router).use(vuetify).mount('#app')
