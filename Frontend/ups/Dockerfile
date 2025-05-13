FROM node:18-alpine

WORKDIR /app

ENV CI=true
ENV PORT=3000
ENV PATH /app/node_modules/.bin:$PATH

COPY package.json ./
COPY package-lock.json ./
RUN npm install --silent
RUN npm install react-scripts -g --silent

COPY . ./

EXPOSE 3000

CMD ["npm", "start"]
