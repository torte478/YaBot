CREATE TABLE public.place (
    id integer NOT NULL,
    name character varying NOT NULL,
    image bytea
);

ALTER TABLE public.place OWNER TO postgres;

CREATE SEQUENCE public.place_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER TABLE public.place_id_seq OWNER TO postgres;

ALTER SEQUENCE public.place_id_seq OWNED BY public.place.id;

ALTER TABLE ONLY public.place ALTER COLUMN id SET DEFAULT nextval('public.place_id_seq'::regclass);

ALTER TABLE ONLY public.place
    ADD CONSTRAINT place_pkey PRIMARY KEY (id);