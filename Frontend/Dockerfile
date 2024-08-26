FROM node:20-alpine AS build

WORKDIR /build

COPY package*.json ./

RUN npm install

COPY . .

RUN npm run build

FROM nginx:alpine

WORKDIR /app

COPY --from=build /build/build .

COPY nginx.conf /etc/nginx/conf.d/default.conf

CMD ["nginx", "-g", "daemon off;"]