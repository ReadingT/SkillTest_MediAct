// import NextAuth, { NextAuthOptions } from "next-auth";
// import CredentialsProvider from "next-auth/providers/credentials";

// export const authOptions: NextAuthOptions = {
//   providers: [
//     CredentialsProvider({
//       name: "Credentials",
//       credentials: {
//         email: { label: "Email", type: "email" },
//         password: { label: "Password", type: "password" },
//       },
//       async authorize(credentials) {
//         // ส่วนนี้คือการยิง Request ไปยัง Backend API ของคุณเพื่อตรวจสอบ Login
//         const res = await fetch("http://your-backend-api.com/api/auth/login", {
//           method: "POST",
//           headers: { "Content-Type": "application/json" },
//           body: JSON.stringify({
//             email: credentials?.email,
//             password: credentials?.password,
//           }),
//         });

//         const user = await res.json();

//         // ถ้า Backend คืนข้อมูล user และ token มา (Login สำเร็จ)
//         if (res.ok && user) {
//           // สิ่งที่ return จากตรงนี้ จะถูกส่งไปที่ callback jwt
//           return user;
//         }
//         // Login ไม่สำเร็จ
//         return null;
//       },
//     }),
//   ],
//   session: {
//     strategy: "jwt", // ใช้ JSON Web Tokens
//   },
//   callbacks: {
//     // Callback นี้จะถูกเรียกหลังจาก authorize สำเร็จ
//     async jwt({ token, user }) {
//       // user object คือข้อมูลที่ได้จาก authorize
//       // เราจะแนบบทบาท (role) และ access token ไปกับ JWT token
//       if (user) {
//         token.role = user.role;
//         token.accessToken = user.token; // สมมติ Backend คืน token มาในชื่อนี้
//       }
//       return token;
//     },
//     // Callback นี้จะถูกเรียกเพื่อสร้าง session object ที่ฝั่ง Client
//     async session({ session, token }) {
//       if (session.user) {
//         session.user.role = token.role as string;
//         session.accessToken = token.accessToken as string;
//       }
//       return session;
//     },
//   },
//   pages: {
//     signIn: "/login", // บอก NextAuth ว่าหน้า Login ของเราคือ /login
//   },
// };

// auth.ts

import NextAuth, { NextAuthOptions } from "next-auth";
import CredentialsProvider from "next-auth/providers/credentials";

export const authOptions: NextAuthOptions = {
  providers: [
    CredentialsProvider({
      name: "Credentials",
      credentials: {
        email: { label: "Email", type: "email" },
        password: { label: "Password", type: "password" },
      },
      async authorize(credentials) {
        // ส่วนนี้คือการตรวจสอบ Login แบบง่ายๆ สำหรับการทดสอบ
        const fixedEmail = "test@example.com";
        const fixedPassword = "password123";

        // ตรวจสอบ email และ password ที่ผู้ใช้กรอกเข้ามา
        if (credentials?.email === fixedEmail && credentials?.password === fixedPassword) {
          // ถ้าข้อมูลตรงกัน ให้คืนค่า user object
          const user = {
            id: "1",
            email: fixedEmail,
            name: "Test User",
            role: "HeadNurse", // กำหนดบทบาทที่ต้องการ
            token: "fake-access-token", // กำหนด token ปลอม
          };
          return user;
        }
        // ถ้าข้อมูลไม่ตรงกัน ให้คืนค่าเป็น null
        return null;
      },
    }),
  ],
  session: {
    strategy: "jwt",
  },
  callbacks: {
    async jwt({ token, user }) {
      if (user) {
        token.role = user.role;
        token.accessToken = user.token;
      }
      return token;
    },
    async session({ session, token }) {
      if (session.user) {
        session.user.role = token.role as string;
        session.accessToken = token.accessToken as string;
      }
      return session;
    },
  },
  pages: {
    signIn: "/login",
  },
};