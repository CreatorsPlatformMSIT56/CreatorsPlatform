export type Member = {
  id: number;
  name: string;
  email: string;
  creatorId: number | null;
  avatar: string | null;
};
export type Lookup = { id: number; name: string; categoryId?: number };
export type Lookups = { categories: Lookup[]; subtitles: Lookup[] };
export type Plan = {
  id: number;
  name: string;
  price: number;
  level: number | null;
  description: string;
};
export type Creator = {
  id: number;
  name: string;
  description: string;
  notice: string;
  bannerUrl: string | null;
  avatar: string;
  followers: number;
  following: boolean;
  plans: Plan[];
};
export type Card = {
  id: number;
  title: string;
  creatorId: number;
  creatorName: string;
  image: string | null;
  likes?: number;
  priceMin?: number;
  priceMax?: number | null;
  subtitle?: string;
  planLevel?: number | null;
};
export type PageData = {
  items: Card[];
  total: number;
  page: number;
  pageSize: number;
};
export type Work = Card & {
  description: string;
  uploadDate: string;
  tags: Lookup[];
  comments: { id: number; text: string; name: string; avatar: string }[];
};
export type Commission = Card & {
  description: string;
  overDate: string | null;
  images: string[];
};
export type EventItem = {
  id: number;
  name: string;
  description: string;
  startDate: string;
  endDate: string;
  creatorId: number;
  creatorName: string;
  categoryId: number;
  banner: string;
  eventStyle: string | null;
};
export type Entry = {
  id: number;
  title: string;
  description: string;
  sample: boolean;
  image: string;
  likes: number;
  creatorId: number;
  creatorName: string;
};
export type EventDetail = { item: EventItem; images: Entry[] };
export type Order = {
  id: number;
  title: string;
  description: string;
  price: number;
  status: string;
  orderDate: string;
  owner: boolean;
};
export type Dashboard = {
  creator: { id: number; description: string; notice: string } | null;
  plans: Plan[];
  works: { id: number; title: string }[];
  commissions: { id: number; title: string }[];
  events: { id: number; name: string }[];
  subscriptions: {
    id: number;
    name: string;
    startDate: string;
    endDate: string;
    creatorId: number;
  }[];
  orders: Order[];
};
