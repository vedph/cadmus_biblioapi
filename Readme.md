# Cadmus Bibliographic API

Note: this is only an experimental STUB project. It will be fully implemented when it's required (deadlines...).

## Overview

This is an API used to expose a minimalist bibliographic database to Cadmus part editors.

The reason behind this project is providing an additional backend to a number of part editors which should share a global bibliographic database, and use references to its items.

Cadmus is mainly focused on producing content in the form of a set of composable, highly structured yet open and modular "documents", which correspond to the Cadmus notion of _item_. Each item contains any number of specialized, independent and reusable models, the _parts_. Using an analogy, you might think of a file system, where Cadmus items are folders, and Cadmus parts are the files inside each folder. Users write edit these files, and each file has its own type and structure. In any moment users can add new files to a folder. The same folder might thus include text documents, spreadsheets, pictures, etc. Each folder has a name, which identifies it, and a set of metadata; apart from this, all the folders are equal; it's what you put inside them which matters. In Cadmus, items are like folders: they have an ID and a set of metadata, and contain any number of parts (files). Each part is a piece of structured data.

This architecture is designed to be open, modular, and easy to use; its main target is content creation, rather than presentation. Thus, usually once content is created it can be variously manipulated by software to generate data optimized for publications; for instance, you might want to build a relational database acting as the data source for a web application publishing some subset of the data. Just like in many software paradigms the data model is different from the view model (a subset of the data restructured in order to provide the optimal source for a UI), here the technologies used for creating data may be different from those used for presenting them.

In most cases, this document-like approach fits the typical content creation process. This is because Cadmus targets scenarios where data models are highly dynamic and modular, and can be changed at any time as the research work progresses. Of course, you can go with more traditional solutions if all your data are already well-defined at the start of the project, and you can be confident in defining and implementing their model as a "closed" structure, which hardly changes in time. In these cases typically you just have to create e.g. a relational database, whose schema is optimized for data normalization and performance, at the price of a more rigid architecture.

## Normalization Levels

Anyway, even in Cadmus there are scenarios where a certain degree of normalization is required. In many cases, it's just a matter of good design and a little convention to deal with them. For instance, say you are building a database with data about all the persons connected to a corpus of letters. In this case, you would have items like letters, persons, manuscripts, etc., each with a number of parts. Some of these parts may be shared across different items (e.g. a part with datation, a part with a generic note, etc.); others instead are more specific. As for persons, you might have several parts which need to reference them: for instance, from a letter's text we might want to reference a person which is cited there, and found in our database. As letters and persons are both items, and each item's part is independent, the easiest way to make such references is sticking to a naming convention, just like you might want to do in a file system. If you were writing a monographic text document about each person, you would store each in a file in a `persons` folder, named after that person (e.g. `petrarca.doc`, `barbato.doc`, etc.). Then, should you need to add a reference to Barbato in the Petrarca document, you would just add the file's name (`barbato`) to refer to its document.

In Cadmus, you can do the same: a part with biographic data about Barbato would also let you enter an arbitrarily chosen, human-readable ID for that person, like `barbato`; another part needing to reference him, e.g. a part listing all the exchanges of manuscripts between Petrarca and Barbato, or a part listing all the pseudonyms used in a letter for various persons, would just address each person with his ID. This intentionally is a loose link between documents: when writing the document about Petrarca, the document about Barbato might even not yet exist; you could anyway use `barbato` to reference him. Using such human-friendly IDs in this scenario is fine, because they are simple to use, understand, and remember, and provide a sort of "weak references". Cadmus can also build lookup lists of similar IDs, and provide it to part editors, so that they can pick an ID from a list rather than typing it (unless it refers to something which is not yet found in the existing data). Of course, in another scenario, where your data are not so various and open to expansion, you might well prefer a relational database, where you can define "strong references" between data, and you also are constrained to them (which can be good or not, according to the data entry processes); but this is not the case of our sample.

This way, you enter all the data you want about each person in a number of different parts under their item; and at any time you can add new parts to enrich the person's data model. When another part needs to refer to a person item, it just picks the ID defined for that person in one of that item's parts.

In other cases you might want to attain a higher level of normalization, or even work with some external data repository. For instance, you might want to provide bibliographic references from an editable, shared bibliographic database; or iconographic references from an external, readonly repository of manuscripts watermarks. In the former case, we're still referencing data inside the domain of our system; in the latter case, we're going to reference data outside it. Yet, the solution in both cases would be the same, thanks to the independence granted to each part in this architecture. Just as the part is an independent data model, its editor UI is an independent piece of software; it gets integrated into the Cadmus framework, but its implementation can be really anything. In this case, we would need to create a part editor which talks to a web service exposing data, either writeable or readonly. Cadmus part editors talk to the Cadmus API service, which reads and writes data from the Cadmus database. In this case, the part editor would talk to an additional service.

For instance, think of a part with biographic data about a person: we might want to add some bibliographic references related to events and works cited in that part. To this end, a reference would just be a conventional citation, built like "Moore 1926", which references a full bibliographic item stored elsewhere using only the author's last name and publication date. In this editor, we would like users wanting to add such a reference to either type it, or be aided in picking it from a list appearing when typing some of the author's letters. For instance, the user just types `moo`, and gets a list of all the bibliographic items whose author's last name match this text. He can then pick the desired item, and have "Moore 1926" automatically inserted as the required reference.

Given that a part editor is just a web "page" with its own HTML-based UI and Javascript code, developers are free to implement any editing logic. The only feature needed here from the framework is getting some parameters, like e.g. the URI to the required service: this could be hardcoded in case of external services, but especially when dealing with in-house developed services, which move according to the environment (development, testing, staging, production...), we have better get the URI from an externally defined parameter. This way, we could e.g. have our custom Docker compose stack including both the Cadmus API service and the bibliography API service, and later move it out of the stack replacing its URL with something else. The Cadmus web editor provides this feature via its `env.js` file, which can be replaced before buliding a Docker image, without recompiling the web app itself. The constants defined there can then be injected into Cadmus components.

## Bibliographic API

This project is an example of such an additional API service, provided for those parts requiring to reference a shared bibliographic repository. As such, its database and API interface are designed to provide just the level of functionality typically required by Cadmus part editors.

### Schema

The bibliographic database here is a MySql database with a very simple schema, centered around the concept of work. Apart from its title and details, the work is connected to these other data:

- a single _work type_, drawn from an editable list (e.g. book, paper, web page, audio recording, TV show...);
- any number of _authors_, with different roles (authors proper have a default role of null; other may be translators, organizations, etc.);
- any number of _keywords_.

Also, works can be contained in a _container_ (e.g. journal, proceedings, books of articles, etc.), with a schema very similar to that of works.

(Paste the following code in the box at [PlantUml website](https://plantuml.com/) if you cannot see it):

```plantuml
@startuml
hide circle
skinparam linetype ortho

entity worktype {
    * id <<PK>>
    * name
}

entity author {
    * id <<PK>>
    * first
    * last
    * lastx
    * suffix
}

entity work {
    * id <<PK>>
    * key
    * title
    * titlex
    * language
    * edition
    * yearPub
    * firstPage
    * lastPage
    --
    * typeId <<FK>>
    * containerId
    * publisher
    * placePub
    * location
    * accessDate
    * note
}

entity container {
    * id <<PK>>
    * key
    * title
    * titlex
    * language
    * edition
    * yearPub
    --
    * typeId <<FK>>
    * publisher
    * placePub
    * location
    * accessDate
    * number
    * note
}

entity keyword {
    * id: number <<PK AI>>
    * language: text
    * value: text
    * valuex: text
}

entity authorwork {
    * authorId <<PK FK>>
    * workId <<PK FK>>
    * role
}

entity authorcontainer {
    * authorId <<PK FK>>
    * workId <<PK FK>>
    * role
}

work }o-- worktype
work }o-- container
authorwork }o-- work
authorwork }o-- author
keywordwork }o-- keyword
keywordwork }o-- work
authorcontainer }o-- work
authorcontainer }o-- author
keywordcontainer }o-- keyword
keywordcontainer }o-- container
@enduml
```

Alternatively, here is a picture:

![schema](./schema.svg)

Some notes about fields:

- some of the fields are duplicated with an `x` suffix: these are the values of the corresponding x-less field, filtered. The API generates these values when storing data, and filters any incoming text used for matching these values. Filtering means that:

  - whitespaces and whitespace sequences are normalized into a single space, and removed from start/end.
  - only letters and digits and apostrophe (`'`) are preserved.
  - all the diacritics are removed, and uppercase letters are lowercased.

- the `key` field is calculated by the backend, by concatenating all the author last names (unfiltered, sorted alphabetically, and separated by an ampersand `&`) and the publication year. Note that the alphabetical sort is required to make the key predictable. Otherwise, we should add more fields to represent their desired order; but this is too costly for such a corner case, so the best choice here is sticking to this convention. This field is used to allow faster lookup by key: clients can thus find the full bibliographic record from its key. When adding/updating a work, if another work with the same key already exists (which is rarely the case, but may happen when the same author(s) publish more than a work in the same year), a letter will be appended to the year (e.g. `Allen 1970b`), up to `z`.

Any key field starting with `!` is treated as a manually managed key, and as such is not automatically updated.

### Repository

The database is managed by a repository, which provides these functionalities:

- works (and containers): get a filtered paged list, get a work from its ID, add or update a work with its full data graph, delete a work.
- TODO

The whole API is modeled on the requirements of a simple and mostly agnostic client, intended to operate in any of these typical ways:

a) lookup:

- lookup a list of _authors_ by typing any part of their last name.
- lookup a list of _works_ by filtering them according to 1 or more properties. There are two matching modes: in the default one, all the properties specified in the filter must be matched; in the other, it is enough to match any of them. You can use the latter when you want the user to find matches in any of the relevant work's fields: e.g. freely type something which could be the author's last name, the work's title, any of the work's keywords, etc. It is up to the client to pick the desired properties to match.
- lookup a list of work _types_: get all of them, or only those containing the specified text in their name.

b) edit:

- _add_ a new work to the repository, together with its authors, type, and keywords.
- _update_ an existing work.
- _delete_ a work from the repository.

So for instance you might have a bibliographic references part, with a list of bibliographic references in the conventional form LastName + publication year. In its editor, you might either type the reference directly if you remember it; or use a lookup to find the work by just typing a few characters, and then pick it from the results.

## Integrating Resources

This project thus provides a centralized and independent bibliographic repository, which gets filled by contributions in the Cadmus editor as the parts are edited. It might start empty, or from some imported data. In any case, this would provide a unified lookup system across several different parts.

The main tasks to integrate a service like this in Cadmus are:

- if required, add the same `AuthenticationController` found in Cadmus API and use the same authentication MongoDB database used by Cadmus to authorize the requests to the API. Configure CORS and authentication as required, and protect the desired API endpoints.

- add a layer to the Docker compose stack, dependent from the Cadmus API (to be able to use its authentication DB); the Cadmus web app depends on it.
