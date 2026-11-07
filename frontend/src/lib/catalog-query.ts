export type SearchParams = Record<string, string | string[] | undefined>;
export function first(value: string | string[] | undefined) { return Array.isArray(value) ? value[0] ?? "" : value ?? ""; }
export function pageNumber(value: string | string[] | undefined) {
  const page = Number(first(value));
  // The API uses 12-item pages and SQL Server's 32-bit Skip offset.
  return Number.isSafeInteger(page) && page > 0 && page <= Math.floor(2147483647 / 12) ? page : 1;
}
