# Config

Ce dossier contient les classes responsables de la gestion des �v�nements dans le plugin.
Chaque classe encapsule un ou plusieurs �v�nements et s'y abonne pour ex�cuter des actions sp�cifiques lorsque ces �v�nements se produisent.
Les classes doivent �tre initialis�es dans la classe EventManager.

## Structure

- Classes : Chaque classe peut encapsuler plusieurs �v�nements du moment que c'est pertinent.
Veuillez utiliser le constructeur pour vous abonner aux �v�nements.
- Propri�t�s : Les propri�t�s des classes doivent �tre pr�c�d�es de commentaires descriptifs et suivre la convention PascalCase.

## Documentation

Pour plus d'informations sur les �v�nements, consultez la documentation officielle :
https://docs.team-nova.fr/guides/D%C3%A9clencheurs/S'abonner%20%C3%A0%20des%20%C3%A9v%C3%A8nements