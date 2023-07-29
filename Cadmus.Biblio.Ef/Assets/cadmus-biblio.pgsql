-- author
CREATE TABLE author (
	id bpchar(36) NOT NULL,
	"first" varchar(50) NOT NULL,
	"last" varchar(50) NOT NULL,
	lastx varchar(50) NOT NULL,
	suffix varchar(50) NULL,
	CONSTRAINT author_pk PRIMARY KEY (id)
);
CREATE INDEX author_last_idx ON author USING btree (last);
CREATE INDEX author_lastx_idx ON author USING btree (lastx);

-- container
CREATE TABLE container (
	id bpchar(36) NOT NULL,
	"key" varchar(300) NOT NULL,
	type_id varchar(20) NULL,
	title varchar(200) NOT NULL,
	titlex varchar(200) NOT NULL,
	"language" bpchar(3) NOT NULL,
	edition int2 NOT NULL,
	publisher varchar(50) NULL,
	year_pub int2 NOT NULL,
	year_pub2 int2 NULL,
	place_pub varchar(100) NULL,
	"location" varchar(500) NULL,
	access_date timestamptz NULL,
	"number" varchar(50) NULL,
	note varchar(500) NULL,
	datation varchar(1000) NULL,
	datation_value double precision NULL,
	CONSTRAINT container_pk PRIMARY KEY (id)
);
CREATE INDEX container_key_idx ON container USING btree (key);
CREATE INDEX container_titlex_idx ON container USING btree (titlex);
CREATE INDEX container_type_id_idx ON container USING btree (type_id);

-- author_container
CREATE TABLE author_container (
	author_id bpchar(36) NOT NULL,
	container_id bpchar(36) NOT NULL,
	"role" varchar(50) NULL,
	ordinal int2 NOT NULL,
	CONSTRAINT author_container_pk PRIMARY KEY (author_id, container_id)
);
-- author_container foreign keys
ALTER TABLE author_container ADD CONSTRAINT author_container_fk FOREIGN KEY (author_id) REFERENCES author(id) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE author_container ADD CONSTRAINT author_container_fk2 FOREIGN KEY (container_id) REFERENCES container(id) ON DELETE CASCADE ON UPDATE CASCADE;

-- work_type
CREATE TABLE work_type (
	id varchar(20) NOT NULL,
	"name" varchar(100) NOT NULL,
	CONSTRAINT work_type_pk PRIMARY KEY (id)
);

-- work
CREATE TABLE "work" (
	id bpchar(36) NOT NULL,
	"key" varchar(300) NOT NULL,
	type_id varchar(20) NULL,
	container_id bpchar(36) NULL,
	title varchar(200) NOT NULL,
	titlex varchar(200) NOT NULL,
	"language" bpchar(3) NOT NULL,
	edition int2 NOT NULL,
	publisher varchar(50) NULL,
	year_pub int2 NOT NULL,
	year_pub2 int2 NULL,
	place_pub varchar(100) NULL,
	"location" varchar(500) NULL,
	access_date timestamptz NULL,
	"number" varchar(50) NULL,
	first_page int2 NOT NULL,
	last_page int2 NOT NULL,
	note varchar(500) NULL,
	datation varchar(1000) NULL,
	datation_value double precision NULL,
	CONSTRAINT work_pk PRIMARY KEY (id)
);
CREATE INDEX work_key_idx ON work USING btree (key);
CREATE INDEX work_titlex_idx ON work USING btree (titlex);
CREATE INDEX work_type_id_idx ON work USING btree (type_id);
-- "work" foreign keys
ALTER TABLE "work" ADD CONSTRAINT work_fk FOREIGN KEY (type_id) REFERENCES work_type(id) ON DELETE SET NULL ON UPDATE CASCADE;
ALTER TABLE "work" ADD CONSTRAINT work_fk_1 FOREIGN KEY (container_id) REFERENCES container(id) ON DELETE CASCADE ON UPDATE CASCADE;

-- author_work
CREATE TABLE author_work (
	author_id bpchar(36) NOT NULL,
	work_id bpchar(36) NOT NULL,
	"role" varchar(50) NULL,
	ordinal int2 NOT NULL,
	CONSTRAINT author_work_pk PRIMARY KEY (author_id, work_id)
);
-- author_work foreign keys
ALTER TABLE author_work ADD CONSTRAINT author_work_fk FOREIGN KEY (author_id) REFERENCES author(id) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE author_work ADD CONSTRAINT author_work_fk_1 FOREIGN KEY (work_id) REFERENCES "work"(id) ON DELETE CASCADE ON UPDATE CASCADE;

-- keyword
CREATE TABLE keyword (
	id serial4 NOT NULL,
	"language" bpchar(3) NOT NULL,
	value varchar(50) NOT NULL,
	valuex varchar(50) NOT NULL,
	CONSTRAINT keyword_pk PRIMARY KEY (id)
);
CREATE INDEX keyword_language_idx ON keyword USING btree (language);
CREATE INDEX keyword_value_idx ON keyword USING btree (value);
CREATE INDEX keyword_valuex_idx ON keyword USING btree (valuex);

-- keyword_container
CREATE TABLE keyword_container (
	keyword_id int4 NOT NULL,
	container_id bpchar(36) NOT NULL,
	CONSTRAINT keyword_container_pk PRIMARY KEY (keyword_id, container_id)
);

-- keyword_work
CREATE TABLE keyword_work (
	keyword_id int4 NOT NULL,
	work_id bpchar(36) NOT NULL,
	CONSTRAINT keyword_work_pk PRIMARY KEY (keyword_id, work_id)
);

-- work_link
CREATE TABLE work_link (
	id serial4 NOT NULL,
	work_id bpchar(36) NOT NULL,
	"scope" varchar(50) NOT NULL,
	value varchar(1000) NOT NULL,
	CONSTRAINT work_link_pk PRIMARY KEY (id),
	CONSTRAINT work_link_fk FOREIGN KEY (work_id) REFERENCES "work"(id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- container_link
CREATE TABLE container_link (
	id serial4 NOT NULL,
	container_id bpchar(36) NOT NULL,
	"scope" varchar(50) NOT NULL,
	value varchar(1000) NOT NULL,
	CONSTRAINT container_link_pk PRIMARY KEY (id),
	CONSTRAINT container_link_fk FOREIGN KEY (container_id) REFERENCES "container"(id) ON DELETE CASCADE ON UPDATE CASCADE
);
