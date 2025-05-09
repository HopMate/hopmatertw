// client/src/components/ui/Badge.tsx
import React from 'react';

interface BadgeProps {
    children: React.ReactNode;
    variant?: 'primary' | 'secondary' | 'danger' | 'success' | 'warning';
    className?: string;
}

const Badge: React.FC<BadgeProps> = ({
    children,
    variant = 'primary',
    className = '',
}) => {
    const baseStyle = 'px-2 py-1 text-xs font-semibold rounded-full';

    const variantStyles = {
        primary: 'bg-blue-100 text-blue-800',
        secondary: 'bg-gray-100 text-gray-800',
        danger: 'bg-red-100 text-red-800',
        success: 'bg-green-100 text-green-800',
        warning: 'bg-yellow-100 text-yellow-800',
    };

    return (
        <span className={`${baseStyle} ${variantStyles[variant]} ${className}`}>
            {children}
        </span>
    );
};

export default Badge;