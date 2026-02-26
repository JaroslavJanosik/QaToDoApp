FROM node:lts-alpine AS build-env

WORKDIR /app
COPY ./package*.json .
RUN npm install
COPY . ./
RUN npm run build

FROM nginx:stable-alpine as prod-env
COPY --from=build-env /app/build /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]