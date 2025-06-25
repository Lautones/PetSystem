# UI

A aplicação frontend está integrada com dois microsserviços especializados: CadastroPet para gestão de registros de animais e AgendamentoCuidados para administração de serviços clínicos. A interface oferece uma experiência intuitiva com funcionalidades completas para cadastro, visualização, busca com filtros específicos, edição e exclusão tanto de pets quanto dos serviços veterinários associados. O sistema foi projetado para garantir usabilidade eficiente, permitindo o gerenciamento simplificado de todas as informações através de uma interface unificada e de fácil navegação, onde os usuários podem alternar facilmente entre a gestão de animais e o agendamento de consultas por meio de uma navbar resposiva.

## Tecnologias Utilizadas
- Node 23
- Angular 19
- CSS3
- HTML5

# CADASTROPET

O microsserviço gerencia o cadastro de pets, armazenando informações detalhadas e integrando imagens por meio das APIs externas [TheCatAPI](https://thecatapi.com/) e [TheDogAPI](https://thedogapi.com/). Além disso, oferece uma funcionalidade de busca avançada por espécie e raça.

A comunicação entre serviços ocorre de forma assíncrona via RabbitMQ. Sempre que um novo pet é cadastrado, uma mensagem é enviada, e o microsserviço também pode responder a solicitações de dados para fornecer informações ao serviço de Agendamento de Cuidados.

Para mais informações, consultar o arquivo "Desafio de Microserviços".

## Tecnologias Utilizadas
- .Net 5
- Entity Framework Core
- Docker
- [RabbitMQ](http://localhost:15672/#/queues)
- MySQL
- Postman
- [Swagger](https://localhost:5001/index.html)

# AGENDAMENTOCUIDADOS

O microsserviço de Agendamento gera automaticamente agendamentos ao registrar um novo pet em 'CadastroPet', aplicando regras de negócio baseadas na idade (recém-nascido ou sênior). Além disso, permite agendamentos manuais para cuidados específicos, como vacinações e check-ups.

Os dados são armazenados no MySQL, enquanto as informações dos pets são obtidas via RabbitMQ, por meio da comunicação com o microsserviço de cadastro de pets.

Para mais informações, consultar o arquivo "Desafio de Microserviços".

## Tecnologias Utilizadas
- .Net 8
- Entity Framework Core
- Docker
- [RabbitMQ](http://localhost:15672/#/queues)
- MySQL
- Postman
- [Swagger](http://localhost:5129/index.html)

# INSTALAÇÃO DOS MICROSERVIÇOS
1. Clone o repositório:

   git clone https://git.gft.com/muan/desafio_angular.git

2. As configurações dos servidores estão no appsettings.json. Escolhi não apagar para facilitar o teste. Então, crie os servers com as mesmas configurações para rodar a aplicação