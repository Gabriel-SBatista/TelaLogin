
# Login

Login e Cadastro de usuarios, com confirmação de email, validação e hash de senha.


## Demonstração

https://www.loom.com/share/1932d218192a4535a396812706d0881a


## Rodar nova migration

dentro da pasta login.api rodar: dotnet ef migrations add [DESCRICAO DA MIGRATION] --project ../Login.Data --startup-project . --output-dir /Migrations

## Mandar migration para o banco

dentro da pasta login.api rodar: dotnet ef database update --project ../Login.Data --startup-project .

## Configurando o appsettings.json

Para rodar esse projeto, você vai precisar de uma connection string para o banco de dados SqlServer e outra para o RabbitMQ:

```json
"ConnectionStrings": {
  "LoginSqlServerConnection": "",
  "RabbitMQ": ""
}
```
Configurar o JWTToken:
```json
"JWTToken": {
  "StringKey": "",
  "JwtIssuer": "",
  "JwtAudience": "",
  "JwtExpiryInDays": 
}
```
Adicionar as credencias do SMTP:
```json
"SMTP": {
  "Server": "",
  "Porta": ,
  "Username": "",
  "Password": ""
}
```

## Fluxograma da arquitetura

![Fluxograma](assets/fluxograma.png)

## Tecnologias e frameworks utilizados

- Argon2: Utilizei o argon2 para fazer o hash da senha, por ja ter usado outras vezes e por ser um algoritimo muito utilizado para hash de senhas, ja que é um dos mais seguros, pois tem a capacidade de usar grandes quantidades de memória e cria um salt para adicionar a senha antes do hash.
- JWTToken: Utilizei o padrao JWT por ser seguro ja que usam HMAC (um segredo compartilhado) ou RSA (um par de chaves pública/privada), que garante que os dados não sejam alterados e por poder ser enviado facilmente entre requisições HTTP.
- SQLServer e EFCore: Utilizei o SQLServer em conjunto com o Entity Framework Core por ser um ambiente onde ja estou bastante acostumado a trabalhar e sei que é seguro e possui um ótimo desempenho.
- MOQ e xUnit: Utilizei o MOQ para simulações de objestos e xUnit para testes unitarios pois ambos são os frameworks mais utilizado de suas categorias em .Net.
- RabbitMQ: Utilizei o RabbitMQ para mensageria pela facilidade e por ja ter trabalhado com ele em outro projeto.
- Fluent Validation: Utilizei Fluent Validation para facilitar e organizar as validações de entrada do usuario
- ASP.NET MVC: Para o front foi utilizado o ASP.NET MVC por ser na minha opinião mais organizado e eficiente, e é um padrão que estou mais acostumado.

## Padrões e funcionalidades
- Utilizei um middleware para tratamento de erros inesperados em conjunto com log para maior facilidade de localização de erros e apresenta-los de forma amigavel ao usuario.
- E foi utilizada tambem a plataforma new relic para a organização e visualização de logs.
- Utilizei os principios do SOLID e alguns padrões como: Padrão Repository, Padrão Service, Injeção de Dependência e Autenticação/Autorização.
- Para os testes de Repository foi utilizado o EF inMemory para armazenar dados de testes na memória RAM.
- Foi utilizado a funcionalidade de health check para monitorar o estado da aplicação.
- O projeto tambem foi organizado usando o padrão de design de arquitetura em camadas.
- E foi utilizado o padrão arquitetural Event Sourcing para emissão de eventos do usuario.

