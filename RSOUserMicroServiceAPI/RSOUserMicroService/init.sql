-- table: public.user

--drop table if exists public."user";

--create table if not exists public."user"
--(
--    "userid" integer not null generated always as identity ( increment 1 start 1 minvalue 1 maxvalue 2147483647 cache 1 ),
--    "username" character varying(100) collate pg_catalog."default" not null,
--    "useremail" character varying(200) collate pg_catalog."default" not null,
--    "userpassword" character varying(300) collate pg_catalog."default" not null,
--    "useraddress" character varying(300) collate pg_catalog."default" not null,
--    "userzipcode" character varying(5) collate pg_catalog."default" default '1000'::character varying,
--    "usercity" character varying(100) collate pg_catalog."default" default 'ljubljana'::character varying,
--	"registeredon" timestamptz,
--    constraint commerceuser_pkey primary key ("userid")
--)

--tablespace pg_default;

--alter table if exists public."user"
--    owner to shanji;