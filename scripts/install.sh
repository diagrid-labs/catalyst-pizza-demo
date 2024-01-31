printf "\n\nHold on, we're installing some tools and building the projects...\n\n"
sudo apt-get update
sudo apt-get install -y xdg-utils
dotnet build ./back-end/PizzaOrderService
dotnet build ./EvilCorpDemo/KitchenService
cd ./front-end
npm install
cd ..
npm i -g vercel
curl -o- https://downloads.diagrid.io/cli/install-catalyst.sh | bash
sudo mv ./diagrid /usr/local/bin 
