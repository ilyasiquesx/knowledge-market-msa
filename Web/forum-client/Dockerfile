FROM node:slim as build
ARG GATEWAY_URL
WORKDIR /app
COPY Web/forum-client/package.json .
COPY Web/forum-client/package-lock.json .
COPY Web/forum-client/tsconfig.json .
RUN npm install

COPY ./Web/forum-client/public ./public/
COPY ./Web/forum-client/src ./src/

RUN REACT_APP_GATEWAY_URL=${GATEWAY_URL} \
  npm run build

FROM nginx:1.17-alpine
COPY --from=build /app/build /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]