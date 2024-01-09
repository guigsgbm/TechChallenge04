# Documentação do Projeto TechChallenge04

## Visão Geral
Este projeto é uma aplicação .NET 8 que consiste em uma API (que funciona como um Producer tambem), e um Worker (Consumer). O guia a seguir fornece instruções detalhadas para configurar e implantar o projeto em um ambiente Azure.

## Requisitos
Antes de começar, certifique-se de ter o seguinte:
- Conta no Azure com as permissões necessárias.
- Conta no Azure DevOps com acesso ao projeto desejado.
- .NET 8 SDK instalado localmente.
- Git instalado localmente.

## Passos para Configuração e Implantação

### 1. Deploy dos recursos no Azure.
- Sql Server + Database (Aplicar Migrations, ex: dotnet ef database update).
- ACR (Azure Container Registry).
- ACI (Azure Container Instance).
- Azure App Service.

### 2. Clone o Repositório

- Abra um terminal e execute o seguinte comando para clonar o repositório do GitHub:

	```
  git clone https://github.com/guigsgbm/TechChallenge04.git
	```

### 3. Build e Deploy dos recursos

- Navegue ate o diretorio raiz do projeto clonado. Ex:
	```
	cd .\TechChallenge04\
	```
- Rode os comandos de Migrations
	```
	dotnet ef migrations add "Initial" -p.\src\Infrastructure\Infrastructure.csproj -c AppDbContext -s.\src\Services\ItemApi\ItemApi.csproj -o.\DB\Migrations\ --verbose
  
	dotnet ef database update -p .\src\Infrastructure\Infrastructure.csproj -c AppDbContext -s .\src\Services\ItemApi\ItemApi.csproj
	```

- Build e push da imagem
  ```
	docker build . -f .\src\Services\ItemApi\Dockerfile -t nomeacr.azurecr.io/itemapi:latest
 
	docker build . -f .\src\Services\CreatedItemsConsumer\Dockerfile -t nomeacr.azurecr.io/createditemsconsumer:latest

	az login
 
	az acr login --name "NomeACR"

	docker push nomeacr.azurecr.io/itemapi:latest

	docker push nomeacr.azurecr.io/createditemsconsumer:latest
	```
	*As imagens foram baseadas para realizar o build em Linux

### 4. Criar e configurar App Services (WebApp)

- Cada imagem tera seu respectivo App Service.
No momento da criacao do App Service, na opcao "Publish", iremos selecionar "Docker Container".
Depois, iremos navegar ate a aba "Docker" e apontaremos para as imagens que foram enviadas ao nosso ACR anteriormente. EX:

	Options = Single Container
	Image Source = Azure Container Registry
	Registry = Seu ACR
	Image = Nome da Imagem (ex: itemapi)
	Tag = latest

### Conclusão

- Ao final, o aplicativo será implantado, assim como todo o resto do fluxo terá sido executado e as aplicacoes estaram disponiveis nos endpoints dos App Services.