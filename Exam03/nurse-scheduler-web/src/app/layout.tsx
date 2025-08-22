// app/layout.tsx

// Import global CSS, if you have one
import './globals.css';

export const metadata = {
  title: 'Nurse Scheduler',
  description: 'Application for nurse schedule management',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body>
        {children}
      </body>
    </html>
  );
}