import 'next-auth';
import 'next-auth/jwt';

// โค้ดส่วนนี้เป็นการ "ขยาย" หรือ "Augmenting" Type เดิมของ NextAuth
// เพื่อให้ TypeScript รู้จัก properties ที่เราเพิ่มเข้าไป

declare module 'next-auth' {
  /**
   * Extends the built-in session.user type
   */
  interface Session {
    accessToken?: string; // เพิ่ม accessToken เข้าไปใน Session
    user?: {
      role?: string; // เพิ่ม role เข้าไปใน session.user
    } & DefaultSession['user'];
  }

  /**
   * Extends the built-in user type
   */
  interface User {
    role?: string;   // เพิ่ม role ให้กับ User object ที่ได้จาก authorize callback
    token?: string;  // เพิ่ม token ให้กับ User object ที่ได้จาก authorize callback
  }
}

declare module 'next-auth/jwt' {
  /**
   * Extends the built-in JWT type
   */
  interface JWT {
    role?: string;        // เพิ่ม role เข้าไปใน token
    accessToken?: string; // เพิ่ม accessToken เข้าไปใน token
  }
}