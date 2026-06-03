# StringHeaven - Loja Virtual 🎸

Bem-vindo ao repositório da **StringHeaven**, uma loja virtual completa especializada em instrumentos musicais, construída com as tecnologias mais modernas do ecossistema .NET (Blazor WebAssembly e API REST) e banco de dados PostgreSQL.

---

## 🚀 Como rodar o projeto no seu PC

Para testar a loja na sua própria máquina, siga este passo a passo simples:

### Pré-requisitos
Antes de começar, você precisa ter instalado no seu computador:
* [SDK do .NET](https://dotnet.microsoft.com/download) (versão mais recente).
* [Docker Desktop](https://www.docker.com/products/docker-desktop) rodando no fundo.

### Passo 1: Ligar o Banco de Dados (Docker)
Nós usamos o Docker para rodar o banco de dados sem precisar instalar nada pesado no seu PC. Abra o seu terminal e rode o comando abaixo para baixar e iniciar o banco da loja:

bash
docker run --name lojavirtual-db -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=AdminLoja@123 -e POSTGRES_DB=lojavirtual_db -p 5432:5432 -d postgres:latest

*(Dica: certifique-se de que a senha no comando seja a mesma que está configurada no arquivo `appsettings.json` do projeto).*

### Passo 2: Criar as Tabelas
Agora precisamos avisar ao sistema para criar as tabelas vazias no banco de dados. Pelo terminal, entre na pasta do projeto principal (`MinhaLoja.web`) e execute:

bash
dotnet ef database update


### Passo 3: Rodar a Aplicação
Com o banco pronto, é só dar a partida no projeto com o comando:

bash
cd MinhaLoja.web
dotnet run

Aguarde alguns segundos e acesse o link que vai aparecer no seu terminal (geralmente `http://localhost:XXXX`). A loja já estará funcionando!

---

## ✨ Funcionalidades da Loja

O sistema foi desenhado para simular o fluxo real de um e-commerce ponta a ponta. Aqui está tudo o que você pode testar:

* **Vitrine de Produtos:** Explore nosso catálogo de instrumentos (guitarras Gibson, Fender, etc.) com preços formatados e descrições detalhadas.
* **Carrinho de Compras Inteligente:** Adicione guitarras ao carrinho. O sistema salva tudo na memória do seu navegador, então você não perde os itens escolhidos mesmo se fechar a aba ou atualizar a página sem querer.
* **Cadastro e Login Seguros:** Crie uma conta no sistema. Utilizamos tokens de segurança para garantir que apenas você tenha acesso aos seus dados.
* **Checkout Rápido (Finalizar Compra):** * **Se você for novo:** Pode criar sua conta e finalizar o pedido diretamente na tela de checkout, sem burocracia.
  * **Se já for cliente:** Ao fazer login, o sistema puxa automaticamente o seu CPF, telefone e endereço salvos, deixando tudo pré-preenchido para você.
* **Minha Conta:** Uma área privada onde você pode editar e salvar suas informações de entrega a qualquer momento.
* **Meus Pedidos:** O histórico completo de compras. Aqui você acompanha a data exata que o pedido foi feito, as guitarras que foram compradas, o valor total da nota e o status do pagamento.

## Swagger http://localhost:XXXX/swagger/index.html
No Swagger você encontrará todas as funcionalidades da Loja. Lá é possível criar uma conta de Admin da seguinte maneira:

**I.** Na parte Auth, basta clicar em /api/Auth/registrar-admin;
**II.** Criada a conta, basta clicar em /api/Auth/login e inserir a conta;
**III.** Um token será gerado, e no canto superior do Auth haverá um cadeado escrito "Authorize";
**IV.** Insira o token sem as aspas dentro de "Authorize" e clique em "Authorize".
**V.** Pronto, agora você tem privilégios de Admin e você pedir editar produtos por completo! Essas funcionalidades somente são vistas pelo Admin;