# Entities

Ce dossier contient toutes les classes représentant des entités en base de données.

## Structure

- Classe : Toutes les classes doivent hériter de `ModEntity<VotreClasse>`.

- Propriétés : Les propriétés publiques doivent suivre la convention PascalCase.
Les champs privés doivent être précédés d'un underscore (_) et suivre la convention camelCase.
Chaque propriété doit être précédée d'un commentaire descriptif et nommée en anglais. 

- Méthodes : Les entités peuvent inclure des méthodes pour manipuler les données ou effectuer des opérations spécifiques.
Chaque méthode doit également être précédée d'un commentaire descriptif.

## Documentation

Pour plus d'informations sur l'utilisation de l'ORM fourni par ModKit, consultez la documentation officielle :
https://github.com/Aarnow/NovaLife_ModKit-Releases/wiki/ORM