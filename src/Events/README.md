# Config

Ce dossier contient les classes responsables de la gestion des événements dans le plugin.
Chaque classe encapsule un ou plusieurs événements et s'y abonne pour exécuter des actions spécifiques lorsque ces événements se produisent.
Les classes doivent être initialisées dans la classe EventManager.

## Structure

- Classes : Chaque classe peut encapsuler plusieurs événements du moment que c'est pertinent.
Veuillez utiliser le constructeur pour vous abonner aux événements.
- Propriétés : Les propriétés des classes doivent être précédées de commentaires descriptifs et suivre la convention PascalCase.

## Documentation

Pour plus d'informations sur les événements, consultez la documentation officielle :
https://docs.team-nova.fr/guides/D%C3%A9clencheurs/S'abonner%20%C3%A0%20des%20%C3%A9v%C3%A8nements