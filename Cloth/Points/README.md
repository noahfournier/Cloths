# Points

Ce dossier regroupe les classes représentant les points bleues.

## Structure

- Classe : Toutes les classes doivent hériter de `ModEntity<VotreClasse>` ainsi que de `PatternData`

- Propriétés : Les propriétés publiques doivent suivre la convention PascalCase.
Les champs privés doivent être précédés d'un underscore (_) et suivre la convention camelCase.
Chaque propriété doit être précédée d'un commentaire descriptif et nommée en anglais. 

- Méthodes : Les points peuvent inclure des méthodes pour effectuer des opérations spécifiques et encapsuler les panels propres au point.
Chaque méthode doit également être précédée d'un commentaire descriptif.

## Conventions de Nommage

- Nom de la Méthode : Le nom de la méthode doit être descriptif et refléter l'objectif. 
S'il s'agit d'une méthode dédié à l'affichage d'un panel, son nom doit toujours se terminer par le mot Panel.
Par exemple, une méthode pour demander définir les sociétés autorisés à intéragir avec le point doit se nommer `SetAllowedBizPanel`.

- Pas d'Imbriquement : Un panel ne doit jamais imbriquer d'autres panels, cela rend le code illisible et difficile à maintenir.

## Documentation

Pour plus d'informations sur la création des points avec ModKit, consultez la documentation officielle :
https://github.com/Aarnow/NovaLife_ModKit-Releases/wiki/PointHelper