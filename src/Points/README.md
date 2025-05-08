# Points

Ce dossier regroupe les classes repr�sentant les points bleues.

## Structure

- Classe : Toutes les classes doivent h�riter de `ModEntity<VotreClasse>` ainsi que de `PatternData`

- Propri�t�s : Les propri�t�s publiques doivent suivre la convention PascalCase.
Les champs priv�s doivent �tre pr�c�d�s d'un underscore (_) et suivre la convention camelCase.
Chaque propri�t� doit �tre pr�c�d�e d'un commentaire descriptif et nomm�e en anglais. 

- M�thodes : Les points peuvent inclure des m�thodes pour effectuer des op�rations sp�cifiques et encapsuler les panels propres au point.
Chaque m�thode doit �galement �tre pr�c�d�e d'un commentaire descriptif.

## Conventions de Nommage

- Nom de la M�thode : Le nom de la m�thode doit �tre descriptif et refl�ter l'objectif. 
S'il s'agit d'une m�thode d�di� � l'affichage d'un panel, son nom doit toujours se terminer par le mot Panel.
Par exemple, une m�thode pour demander d�finir les soci�t�s autoris�s � int�ragir avec le point doit se nommer `SetAllowedBizPanel`.

- Pas d'Imbriquement : Un panel ne doit jamais imbriquer d'autres panels, cela rend le code illisible et difficile � maintenir.

## Documentation

Pour plus d'informations sur la cr�ation des points avec ModKit, consultez la documentation officielle :
https://github.com/Aarnow/NovaLife_ModKit-Releases/wiki/PointHelper