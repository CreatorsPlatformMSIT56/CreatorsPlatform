import type { NextConfig } from "next";

const backend = process.env.BACKEND_URL || "http://localhost:5251";
const config: NextConfig = {
  async rewrites() {
    return ["/api/:path*", "/images/:path*", "/img/:path*", "/favicon.ico"].map(source => ({
      source, destination: `${backend}${source}`,
    }));
  },
};
export default config;
