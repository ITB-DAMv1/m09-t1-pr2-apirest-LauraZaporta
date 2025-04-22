## DOCUMENTACIÓ:

### SOLUCIÓ PROPOSADAA:

**La solució proposada consisteix de dos projectes:**

- _API:_ La part que funciona com a servidor. Serveix per manipular informació (obtenir, modificar, inserir i esborrar dades) sobre els jocs i usuaris de la BBDD i té la possibilitat d'establir connexió amb altres serveis (per exemple, el client). 
  > Controllers: Els controladors que contenen els mètodes http per manipular informació.
  > DTOs: Els objectes que transmeten dades de manera eficaç dintre el programa.
  > Data: La carpeta on es troba l'ApplicationDbContext (context de la base de dades)
  > Hubs: Aquí és present el XatHub per la connexió amb Live Chat.
  > Migrations: Generada automàticament per Entity Framework quan es fa la BBDD.
  > Model: On es troben els models per generar les taules a la BBDD
  > Properties: Configuració d'start de l'API.
  > Tools: On es defineixen els rols.
  > wwwroot/images: Carpeta on s'emmaagatzemen i les imatges dels jocs.
  > Altres arxius: Program.cs i appsettings.json, que són el motor de tota l'api.

- _Client:_ La secció de front-end, on mitjançant Razor Pages es veuen reflexades les funcions més destacades de l'API.
  > DTOs: Amb la mateixa funció que a l'API i especialment útils per rebre respostes d'aquesta sense majors problemes.
  > Pages: On es troba la part View i Model de VMC. Les pàgines i els models d'aquestes que es connecten amb la api per oferir interacció de l'usuari amb les dades d'aquesta.
  > Properties: Similars a les properties de l'API.
  > Tools: En aquesta carpeta es troba TokenHelper amb un mètode que determina si hi ha token i si és vàlid.
  > wwwroot: Carpeta autogenerada però manipulable, sobretot pel css.
  > Altres archius: Exactament igual que a l'API, Program.cs i appsettings.json, que fan funcionar tot el client.

- _Estratègies i procés:_ Primer he optat per desenvolupar al 100% l'API, i com a servei individual funcionava de manera satisfactòria. Pensava que, una vegada desenvolupada l'api, el client seria bufar i fer ampolles i no ha sigut així.
Sobretot hi havia molts errors persistents que al final i per sort he acabat solucionant. En el procés també he hagut de modificar algunes parts de l'API i, sense pensar-hi massa, he fet un mix de branques. Fins i tot he perdut
una part del codi (la pàgina de details) per un reinici automàtic de l'ordinador. Tot i que el control de versions sembli desorganitzat (que ho és). El codi present a la solució és perfectament funcional.


### DIAGRAMA BBDD:

Amb les llibreries utilitzades, identity fa pràcticament tot el treball en quant als usuaris. Dintre de la base de dades genera i gestiona els usuaris, els seus rols, els seus tokens, entre altres. El que nosaltres fem manualment al context
del programa és linkar la nostra taula de jocs amb els usuaris d'Identity, creant així una relació molts-molts que estableix una nova taula que controla les votacions dels jocs.

![image](https://github.com/user-attachments/assets/77d1f11f-9261-4b97-b5ad-731b23de3f9b)

### BIBLIOGRAFIA:
- Documentació C#: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.regularexpressionattribute?view=net-9.0
- Més documentació C#: https://learn.microsoft.com/es-es/dotnet/api/system.io.file.delete?view=net-8.0
- Documentació SQL: https://www.w3schools.com/sql/sql_delete.asp
- Dataedo, (no es pot veure la data exacte, fa 6 anys de la publicació aprox.) Create a database diagram with MS SQL Server Management Studio. 
  Youtube. Recuperat el 22/4/2025 de https://www.youtube.com/watch?v=XRYMS4bt1Pw
- Carrillo Juan, (2/12/2020) Git Clone Branch – Cómo clonar una branch específica. freeCodeCamp. Recuperat el 22/4/2025 de https://www.freecodecamp.org/espanol/news/como-clonar-una-branch-en-git/
- > Mètodes com la votació o l'eliminació els passaré després a un client. Hauria de fer un mètode que busqui i retorni la id per nom del joc o ja m'ocuparé d'això al client?
  > ChatGPT: Gran pregunta! 🔍

Quan passes aquestes accions (com votar o eliminar jocs) al client, et toca decidir:

Ha de passar el nom del joc o la seva id?

🎯 Resposta ràpida:
És millor que el client treballi amb la id del joc.
Això et dona més seguretat, més rapidesa, i evita ambigüitats.

💡 Per què és millor usar la id al client:
✅ 1. La id és única
Dos jocs poden tenir el mateix títol ("Super Game"), però només una id única.

✅ 2. Evites crides extra a l'API
Si el client només té el nom, ha de fer una crida per buscar la id abans d'eliminar o votar. Això són 2 peticions en lloc d'1.

✅ 3. És el patró habitual amb REST APIs
A les APIs REST, la ruta sol contenir la id, ex:

- Altres eines: https://jwt.io/

#### ANOTACIONS:

Funcionalitats parcials o faltants: Xat en viu complert (L'he estat intentant tres hores però ha sigut impossible arreglar-lo fins i tot amb loggers, debugging i ajuda de la IA. Només vull veure una solució i entendre el codi amb el token referenciat a javascript. Terrible), data-seed (no m'ha donat temps), documentació auto-generada en format XML i bona gestió de les branques a git (amb l'aigua al coll he ignorat bastant aquesta bona pràctica tant important :c)
