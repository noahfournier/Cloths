# Panels

Ce dossier regroupe les m�thodes permettant d'ouvrir des panels. Chaque m�thode est responsable de l'ouverture d'un seul panel et doit suivre les conventions.

## Conventions de Nommage

- Nom de la M�thode : Le nom de la m�thode doit �tre descriptif et refl�ter l'objectif du panel. 
Son nom doit toujours se terminer par le mot Panel.
Par exemple, une m�thode pour demander la permission d'assommer un autre joueur pourrait �tre nomm�e `KnockoutRequestPanel`.

- Pas d'Imbriquement : Un panel ne doit jamais imbriquer d'autres panels, cela rend le code illisible et difficile � maintenir.

## Documentation

Pour plus d'informations sur l'utilisation des panels avec ModKit, consultez la documentation officielle :
https://github.com/Aarnow/NovaLife_ModKit-Releases/wiki/PanelHelper