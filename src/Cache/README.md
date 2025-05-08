# Cache

Ce dossier vise à conserver certaines informations provenant de la base de données afin de limiter les requêtes vers celle-ci.
En utilisant un cache, nous réduisons la charge sur la base de données et améliorons les performances du plugin.

## Structure

- Classes : Pour chaque entité que vous souhaitez mettre en cache, créez une nouvelle classe portant le nom de l'entité et se terminant par le mot "Cache".
Cette classe doit contenir une propriété de type Dictionary<int, VotreEntité> qui stockera les données.
- Propriétés : Les propriétés des classes doivent être précédées de commentaires descriptifs et suivre la convention PascalCase.