# Panels

Ce dossier regroupe les méthodes permettant d'ouvrir des panels. Chaque méthode est responsable de l'ouverture d'un seul panel et doit suivre les conventions.

## Conventions de Nommage

- Nom de la Méthode : Le nom de la méthode doit être descriptif et refléter l'objectif du panel. 
Son nom doit toujours se terminer par le mot Panel.
Par exemple, une méthode pour demander la permission d'assommer un autre joueur pourrait être nommée `KnockoutRequestPanel`.

- Pas d'Imbriquement : Un panel ne doit jamais imbriquer d'autres panels, cela rend le code illisible et difficile à maintenir.

## Documentation

Pour plus d'informations sur l'utilisation des panels avec ModKit, consultez la documentation officielle :
https://github.com/Aarnow/NovaLife_ModKit-Releases/wiki/PanelHelper