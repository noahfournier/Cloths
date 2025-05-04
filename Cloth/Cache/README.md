# Cache

Ce dossier vise � conserver certaines informations provenant de la base de donn�es afin de limiter les requ�tes vers celle-ci.
En utilisant un cache, nous r�duisons la charge sur la base de donn�es et am�liorons les performances du plugin.

## Structure

- Classes : Pour chaque entit� que vous souhaitez mettre en cache, cr�ez une nouvelle classe portant le nom de l'entit� et se terminant par le mot "Cache".
Cette classe doit contenir une propri�t� de type Dictionary<int, VotreEntit�> qui stockera les donn�es.
- Propri�t�s : Les propri�t�s des classes doivent �tre pr�c�d�es de commentaires descriptifs et suivre la convention PascalCase.